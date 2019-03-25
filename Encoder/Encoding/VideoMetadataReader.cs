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

      private static readonly object _keyframesLock = new object();
      private static readonly List<double> _keyframes = new List<double>();
      public static bool GetKeyframes( string file, out List<double> keyframes )
      {
         lock ( _keyframesLock )
         {
            // Sync output redirect fails at large number of keyframes
            // so use async output (event based) redirect
            using ( var process = GetProcess( KeyframeArgs( file ) ) )
            {
               process.StartAsChildProcess();
               process.OutputDataReceived += OnKeyframeReceived;
               process.BeginOutputReadLine();
               process.WaitForExit();

               process.OutputDataReceived -= OnKeyframeReceived;
            }

            keyframes = null;
            try
            {
               if ( !_keyframes.Any() )
               {
                  return false;
               }

               keyframes = new List<double>( _keyframes );
               _keyframes.Clear();

               keyframes.Sort();
               return true;
            }
            catch
            {
               return false;
            }
         }
      }

      private static void OnKeyframeReceived( object sender, DataReceivedEventArgs e )
      {
         if ( e.Data != null )
         {
            _keyframes.Add( double.Parse( e.Data ) );
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
