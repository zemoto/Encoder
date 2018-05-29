using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace Interpolator.Encoding
{
   internal sealed class FfmpegEncoder
   {
      private static readonly string _ffmpegExeLocation;

      private readonly EncodingParameters _params;

      private Process _currentffmpegProcess = null;

      public event EventHandler<EncodingProgressEventArgs> EncodingProgress;

      static FfmpegEncoder()
      {
         var executingDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
         _ffmpegExeLocation = Path.Combine( executingDir, "ffmpeg.exe" );
      }

      public FfmpegEncoder( EncodingParameters encodingParams )
      {
         _params = encodingParams;
      }

      private static string BasicArgs( string file ) => $"-hide_banner -i \"{file}\"";
      private string InterpolationArgs => $"-filter:v \"minterpolate='fps={_params.TargetFrameRate}:mi_mode=mci:mc_mode=aobmc:vsbmc=1'\"";
      private const string ReencodeArgs = "-c:v libx264";
      private string EncodingArgs => $"{BasicArgs( _params.SourceFile )} -crf 18 -preset slow {(_params.ShouldInterpolate ? InterpolationArgs : ReencodeArgs)} \"{_params.TargetFile}\"";

      public void StartEncoding( CancellationToken token )
      {
         if ( _currentffmpegProcess != null )
         {
            throw new InvalidOperationException( "Interpolation already started" );
         }

         _currentffmpegProcess = CreateFfmpegProcess( EncodingArgs );

         _currentffmpegProcess.ErrorDataReceived += OnErrorDataReceived;
         _currentffmpegProcess.Exited += CleanupProcessInfo;
         _currentffmpegProcess.Start();
         _currentffmpegProcess.BeginErrorReadLine();
         token.Register( () => _currentffmpegProcess?.Kill() );
      }

      public static bool GetVideoInfo( string file, out double frameRate, out TimeSpan duration )
      {
         var process = CreateFfmpegProcess( BasicArgs( file ) );

         process.Start();
         process.WaitForExit();
         var output = process.StandardError.ReadToEnd();

         try
         {
            var match = Regex.Match( output, "[0-9]+ fps" );
            frameRate = double.Parse( match.Groups[0].Value.Replace( " fps", "" ) );

            match = Regex.Match( output, "Duration: [0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{2}" );
            duration = TimeSpan.Parse( match.Groups[0].Value.Substring( 10 ) );

            return true;
         }
         catch
         {
            frameRate = 0;
            duration = new TimeSpan();
            return false;
         }
         finally
         {
            process.Dispose();
         }
      }

      private void OnErrorDataReceived( object sender, DataReceivedEventArgs e )
      {
         if ( e.Data == null )
         {
            return;
         }

         var match = Regex.Match( e.Data, "frame=[ ]*[0-9]+");
         if ( match.Success )
         {
            var numMatch = Regex.Match( match.Groups[0].Value, @"\d+" );
            var framesDone = int.Parse( numMatch.Groups[0].Value );
            var progress = framesDone / (double)_params.TargetTotalFrames * 100;
            EncodingProgress?.Invoke( this, new EncodingProgressEventArgs( progress ) );
         }
      }

      private void CleanupProcessInfo( object sender, EventArgs e )
      {
         if ( _currentffmpegProcess != null )
         {
            _currentffmpegProcess.ErrorDataReceived -= OnErrorDataReceived;
            _currentffmpegProcess.Exited -= CleanupProcessInfo;
            _currentffmpegProcess.CancelErrorRead();
            _currentffmpegProcess.Dispose();
            _currentffmpegProcess = null;
         }
      }

      public void AwaitCompletion()
      {
         _currentffmpegProcess?.WaitForExit();
      }

      private static Process CreateFfmpegProcess( string arguments )
      {
         return new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = _ffmpegExeLocation,
               Arguments = arguments,
               UseShellExecute = false,
               RedirectStandardError = true,
               CreateNoWindow = true,
               WindowStyle = ProcessWindowStyle.Hidden
            },
            EnableRaisingEvents = true
         };
      }
   }
}
