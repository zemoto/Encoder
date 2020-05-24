using System;
using System.Diagnostics;
using Encoder.ffmpeg;
using ZemotoCommon.Utils;

namespace Encoder.Encoding
{
   internal sealed class VideoMetadata
   {
      public double FrameRate { get; }
      public TimeSpan Duration { get; }
      public int BitRate { get; }

      public VideoMetadata( double frameRate, TimeSpan duration, int bitRate )
      {
         FrameRate = frameRate;
         Duration = duration;
         BitRate = bitRate;
      }
   }

   internal static class VideoMetadataReader
   {
      private static readonly string FfprobeExeLocation;

      private static string VideoInfoArgs( string fileName ) => $"-v error -select_streams v:0 -of default=noprint_wrappers=1:nokey=1 -show_entries stream=r_frame_rate,duration,bit_rate \"{fileName}\"";

      static VideoMetadataReader()
      {
         FfprobeExeLocation = EmbeddedFfmpegManager.GetFfprobeExecutableFilePath();
      }

      public static VideoMetadata GetVideoMetadata( string file )
      {
         var process = GetProcess( VideoInfoArgs( file ) );
         process.StartAsChildProcess();
         process.WaitForExit();

         var output = process.StandardOutput.ReadToEnd();
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

            int.TryParse( values[2], out int bitRate );

            return new VideoMetadata( frameRate, duration, bitRate );
         }
         catch
         {
            return null;
         }
         finally
         {
            process.Dispose();
         }
      }

      private static Process GetProcess( string args ) => new Process
      {
         StartInfo = new ProcessStartInfo
         {
            FileName = FfprobeExeLocation,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
         }
      };
   }
}
