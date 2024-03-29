using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Encoder.Encoding.Tasks;
using Encoder.ffmpeg;
using ZemotoCommon;

namespace Encoder.Encoding
{
   internal sealed class FfmpegEncoder : IDisposable
   {
      private const string ErrorIndicator = "[error]";
      private string BasicArgs => $"-hide_banner -loglevel level -i \"{_encodingTask.SourceFile}\"";
      private static string QualityArgs => "-crf 18 -preset slow -pix_fmt yuv420p -x264opts colormatrix=bt709 -movflags +faststart";
      private string EncodingArgs => $"{BasicArgs} {QualityArgs} {_encodingTask.EncodingArgs} \"{_encodingTask.TargetFile}\"";

      private readonly EncodingTask _encodingTask;

      private Process _currentffmpegProcess;

      public string Error { get; private set; }

      public FfmpegEncoder( EncodingTask encodingTask )
      {
         _encodingTask = encodingTask;
      }

      public void StartEncoding( CancellationToken token )
      {
         var arguments = EncodingArgs;
         Debug.WriteLine( $"Starting ffmpeg with: \"{arguments}\"" );
         _currentffmpegProcess = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = FfmpegUtils.FfmpegFilePath,
               Arguments = arguments,
               UseShellExecute = false,
               RedirectStandardError = true,
               CreateNoWindow = true,
               WindowStyle = ProcessWindowStyle.Hidden
            },
            EnableRaisingEvents = true
         };

         _currentffmpegProcess.ErrorDataReceived += OnEncodingProgress;
         _currentffmpegProcess.Exited += CleanupProcessInfo;
         _currentffmpegProcess.StartAsChildProcess();
         _currentffmpegProcess.BeginErrorReadLine();
         _ = token.Register( () => _currentffmpegProcess?.Kill() );

         _encodingTask.Started = true;
      }

      private void OnEncodingProgress( object sender, DataReceivedEventArgs e )
      {
         if ( e.Data?.Contains( ErrorIndicator ) == true )
         {
            if ( !string.IsNullOrEmpty( Error ) )
            {
               Error += Environment.NewLine;
            }

            Error += e.Data.Replace( ErrorIndicator, string.Empty ).Trim();
            return;
         }

         if ( !_encodingTask.Started || _encodingTask.CancelToken.IsCancellationRequested )
         {
            return;
         }

         if ( e.Data != null )
         {
            var match = Regex.Match( e.Data, "frame=[ ]*[0-9]+");
            if ( match.Success )
            {
               var numMatch = Regex.Match( match.Groups[0].Value, @"\d+" );
               _encodingTask.FramesDone = int.Parse( numMatch.Groups[0].Value );
            }
         }
      }

      private void CleanupProcessInfo( object sender, EventArgs e )
      {
         if ( _currentffmpegProcess != null )
         {
            _currentffmpegProcess.ErrorDataReceived -= OnEncodingProgress;
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

      public void Dispose()
      {
         _currentffmpegProcess?.Dispose();
      }
   }
}
