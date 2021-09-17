using System;
using System.Diagnostics;
using Encoder.ffmpeg;
using ZemotoUtils;

namespace Encoder.Encoding
{
   internal static class VideoMetadataReader
   {
      private static string FFprobeExeLocation;
      private static string FFmpegExeLocation;

      private static string VideoInfoArgs( string fileName ) => $"-v error -select_streams v:0 -of default=noprint_wrappers=1:nokey=1 -show_entries stream=r_frame_rate,duration,bit_rate \"{fileName}\"";
      private static string CropDetectArgs( string fileName, double frameTime ) => $"-hide_banner -ss {frameTime} -i \"{fileName}\" -vframes 2 -vf cropdetect -f null -";

      public static VideoMetadata GetVideoMetadata( string file )
      {
         var output = RunFFProbeProcess( VideoInfoArgs( file ) );
         try
         {
            if ( string.IsNullOrEmpty( output ) )
            {
               return null;
            }

            var values = output.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );

            if ( values.Length != 3 )
            {
               return null;
            }

            var fpsFraction = values[0].Split( '/' );
            double frameRate = Math.Round( double.Parse( fpsFraction[0] ) / double.Parse( fpsFraction[1] ), 2 );

            var duration = TimeSpan.Zero;
            if ( double.TryParse( values[1], out double secondsDuration ) )
            {
               duration = TimeSpan.FromSeconds( secondsDuration );
            }

            uint.TryParse( values[2], out uint bitRate );

            return new VideoMetadata( frameRate, duration, bitRate );
         }
         catch
         {
            return null;
         }
      }


      public static CropDetect GetCropDetect( string file, TimeSpan duration )
      {
         const int numberOfChecks = 5;

         var totalSeconds = duration.TotalSeconds;
         var startEndPadding = totalSeconds / 10;
         var timeToCheck = startEndPadding;
         var timeBetweenChecks = ( ( totalSeconds - ( startEndPadding * 2 ) ) / numberOfChecks );

         CropDetect bestCropDetect = null;
         try
         {
            while ( timeToCheck < totalSeconds )
            {
               var output = RunFFmpegProcess( CropDetectArgs( file, timeToCheck ) );
               var cropDetect = CropDetect.CreateFromString( output );
               if ( cropDetect == null )
               {
                  continue;
               }

               bestCropDetect = bestCropDetect != null ? CropDetect.SelectSmallestCrop( bestCropDetect, cropDetect ) : cropDetect;

               timeToCheck += timeBetweenChecks;
            }

            return bestCropDetect;
         }
         catch
         {
            return null;
         }
      }

      private static string RunFFProbeProcess( string args )
      {
         if ( string.IsNullOrEmpty( FFprobeExeLocation ) )
         {
            FFprobeExeLocation = EmbeddedFfmpegManager.GetFfprobeExecutableFilePath();
         }

         var process = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = FFprobeExeLocation,
               Arguments = args,
               UseShellExecute = false,
               RedirectStandardOutput = true,
               CreateNoWindow = true,
               WindowStyle = ProcessWindowStyle.Hidden
            }
         };

         process.StartAsChildProcess();
         process.WaitForExit();

         var output = process.StandardOutput.ReadToEnd();
         process.Dispose();

         return output;
      }

      private static string RunFFmpegProcess( string args )
      {
         if ( string.IsNullOrEmpty( FFmpegExeLocation ) )
         {
            FFmpegExeLocation = EmbeddedFfmpegManager.GetFfmpegExecutableFilePath();
         }

         var process = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = FFmpegExeLocation,
               Arguments = args,
               UseShellExecute = false,
               RedirectStandardOutput = true,
               RedirectStandardError = true,
               CreateNoWindow = true,
               WindowStyle = ProcessWindowStyle.Hidden
            }
         };

         process.StartAsChildProcess();
         process.WaitForExit();

         string output = process.StandardError.ReadToEnd();
         process.Dispose();

         return output;
      }
   }
}