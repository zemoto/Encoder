using System;
using System.Diagnostics;
using System.Linq;
using Encoder.ffmpeg;
using ZemotoCommon.Utils;

namespace Encoder.Encoding
{
   internal sealed class VideoMetadata
   {
      public double FrameRate { get; }
      public TimeSpan Duration { get; }
      public uint BitRate { get; }

      public VideoMetadata( double frameRate, TimeSpan duration, uint bitRate )
      {
         FrameRate = frameRate;
         Duration = duration;
         BitRate = bitRate;
      }
   }

   internal sealed class CropDetect
   {
      private const string CropDetectFlag = "[Parsed_cropdetect";
      private const string CropFlag = "crop=";

      public int Width { get; }
      public int Height { get; }
      public int X { get; }
      public int Y { get; }

      private CropDetect( int width, int height, int x, int y )
      {
         Width = width;
         Height = height;
         X = x;
         Y = y;
      }

      public static CropDetect CreateFromString( string value )
      {
         var cropDetectData = value.Replace( "\r", string.Empty ).Split( '\n' ).Where( x => x.StartsWith( CropDetectFlag ) ).FirstOrDefault();
         if ( !string.IsNullOrEmpty( cropDetectData ) )
         {
            var cropData = cropDetectData.Split( ' ' ).Where( x => x.StartsWith( CropFlag ) ).FirstOrDefault();
            if ( !string.IsNullOrEmpty( cropData ) )
            {
               var parameters = cropData.Replace( CropFlag, string.Empty ).Split( ':' );
               if ( parameters.Count() == 4 )
               {
                  int width = int.Parse( parameters[0] );
                  int height = int.Parse( parameters[1] );
                  int x = int.Parse( parameters[2] );
                  int y = int.Parse( parameters[3] );

                  return new CropDetect( width, height, x, y );
               }
            }
         }

         return null;
      }

      public static CropDetect SelectSmallestCrop( CropDetect first, CropDetect second )
      {
         int width = Math.Min( first.Width, second.Width );
         int height = Math.Min( first.Height, second.Height );
         int x = Math.Min( first.X, second.X );
         int y = Math.Min( first.Y, second.Y );

         return new CropDetect( width, height, x, y );
      }
   }

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
