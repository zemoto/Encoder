using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Encoder.ffmpeg;
using ZemotoCommon.Utils;

namespace Encoder.Encoding
{
   internal static class VideoMetadataReader
   {
      private static readonly string FfprobeExeLocation;

      private static string VideoInfoArgs( string fileName ) => $"-v error -select_streams v:0 -of default=noprint_wrappers=1:nokey=1 -show_entries stream=r_frame_rate,duration \"{fileName}\"";
      private static string KeyframeArgs( string fileName ) => $"-loglevel error -skip_frame nokey -select_streams v:0 -show_entries frame=pkt_pts_time -of csv=print_section=0 \"{fileName}\"";

      static VideoMetadataReader()
      {
         FfprobeExeLocation = EmbeddedFfmpegManager.GetFfprobeExecutableFilePath();
      }

      public static bool GetVideoInfo( string file, out double frameRate, out TimeSpan duration )
      {
         duration = new TimeSpan();
         frameRate = 0;

         var process = GetProcess( VideoInfoArgs( file ) );
         process.StartAsChildProcess();
         process.WaitForExit();

         var output = process.StandardOutput.ReadToEnd();
         try
         {
            if ( string.IsNullOrEmpty( output ) )
            {
               return false;
            }

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

      public static bool GetKeyframes( string file, out IEnumerable<double> keyFrames )
      {
         keyFrames = null;

         var process = GetProcess( KeyframeArgs( file ) );
         process.StartAsChildProcess();
         process.WaitForExit();

         var output = process.StandardOutput.ReadToEnd();
         try
         {
            if ( string.IsNullOrEmpty( output ) )
            {
               return false;
            }

            keyFrames = output.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Select( x => double.Parse( x ) ).ToList();
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
