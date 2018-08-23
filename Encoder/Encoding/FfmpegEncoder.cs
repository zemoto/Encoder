using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Encoder.Utils;

namespace Encoder.Encoding
{
   internal sealed class FfmpegEncoder
   {
      private string BasicArgs => $"-hide_banner -i \"{_encodingTask.SourceFile}\"";
      private const string DefaultArgs = "-c:v libx264";
      private const string QualityArgs = "-crf 18 -preset slow";
      private string EncodingArgs => $"{BasicArgs} {QualityArgs} {_encodingTask.GetEncodingArguments() ?? DefaultArgs} \"{_encodingTask.TargetFile}\"";

      private static readonly string FfmpegExeLocation;

      private readonly EncodingTaskViewModel _encodingTask;

      private Process _currentffmpegProcess;
      private ProcessCpuMonitor _cpuUsageMonitor;

      static FfmpegEncoder()
      {
         var executingDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
         FfmpegExeLocation = Path.Combine( executingDir, "ffmpeg.exe" );
      }

      public FfmpegEncoder( EncodingTaskViewModel encodingTask )
      {
         _encodingTask = encodingTask;
      }

      public void StartEncoding( CancellationToken token )
      {
         _currentffmpegProcess = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = FfmpegExeLocation,
               Arguments = EncodingArgs,
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
         token.Register( () => _currentffmpegProcess?.Kill() );

         _cpuUsageMonitor = new ProcessCpuMonitor( _currentffmpegProcess );

         _encodingTask.Started = true;
      }

      private void OnEncodingProgress( object sender, DataReceivedEventArgs e )
      {
         if ( !_encodingTask.Started || _encodingTask.Finished )
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
         
         _encodingTask.CpuUsage = _cpuUsageMonitor.GetCurrentCpuUsage();
      }

      private void CleanupProcessInfo( object sender, EventArgs e )
      {
         if ( _cpuUsageMonitor != null )
         {
            _cpuUsageMonitor.Dispose();
            _cpuUsageMonitor = null;
         }
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
   }
}
