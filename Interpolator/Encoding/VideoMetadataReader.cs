using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal static class VideoMetadataReader
   {
      private static readonly string _ffprobeExeLocation;

      private static string VideoInfoArgs( string fileName ) => $"-v error -select_streams v:0 -of default=noprint_wrappers=1:nokey=1 -show_entries stream=r_frame_rate,duration \"{fileName}\"";

      static VideoMetadataReader()
      {
         var executingDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
         _ffprobeExeLocation = Path.Combine( executingDir, "ffprobe.exe" );
      }

      public static bool GetVideoInfo( string file, out double frameRate, out TimeSpan duration )
      {
         duration = new TimeSpan();
         frameRate = 0;

         var process = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = _ffprobeExeLocation,
               Arguments = VideoInfoArgs( file ),
               UseShellExecute = false,
               RedirectStandardOutput = true,
               CreateNoWindow = true,
               WindowStyle = ProcessWindowStyle.Hidden
            }
         };

         process.StartAsChildProcess();
         process.WaitForExit();

         var output = process.StandardOutput.ReadToEnd();
         try
         {
            var values = output.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );

            var fpsFraction = values[0].Split( '/' );
            frameRate = Math.Round( double.Parse( fpsFraction[0] ) / double.Parse( fpsFraction[1] ), 2 );

            if ( double.TryParse( values[1], out double secondsDuration ) )
            {
               duration = TimeSpan.FromSeconds( secondsDuration );
            }
            return true;
         }
         catch
         {
            return false;
         }
         finally
         {
            process.Dispose();
         }
      }
   }
}
