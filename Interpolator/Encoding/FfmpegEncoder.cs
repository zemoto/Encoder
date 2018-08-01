using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal sealed class FfmpegEncoder
   {
      private string BasicArgs => $"-hide_banner -i \"{_encodingTask.SourceFile}\"";
      private string InterpolationArgs => $"-filter:v \"minterpolate='fps={_encodingTask.TargetFrameRate}:mi_mode=mci:mc_mode=aobmc:me_mode=bidir'\"";
      private const string ReencodeArgs = "-c:v libx264";
      private const string QualityArgs = "-crf 18 -preset slow";
      private string EncodingArgs => $"{BasicArgs} {QualityArgs} {( _encodingTask.ShouldInterpolate ? InterpolationArgs : ReencodeArgs )} \"{_encodingTask.TargetFile}\"";

      private static readonly string _ffmpegExeLocation;

      private readonly EncodingTaskViewModel _encodingTask;

      private Process _currentffmpegProcess = null;
      private PerformanceCounter _cpuUsageCounter = null;

      public event EventHandler<EncodingProgressEventArgs> EncodingProgress;

      static FfmpegEncoder()
      {
         var executingDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
         _ffmpegExeLocation = Path.Combine( executingDir, "ffmpeg.exe" );
      }

      public FfmpegEncoder( EncodingTaskViewModel encodingTask )
      {
         _encodingTask = encodingTask;
      }

      public void StartEncoding( CancellationToken token )
      {
         if ( _currentffmpegProcess != null )
         {
            throw new InvalidOperationException( "Interpolation already started" );
         }

         _currentffmpegProcess = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               FileName = _ffmpegExeLocation,
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

         _cpuUsageCounter = new PerformanceCounter( "Process", "% Processor Time", _currentffmpegProcess.ProcessName, true );
      }

      private void OnEncodingProgress( object sender, DataReceivedEventArgs e )
      {
         if ( _currentffmpegProcess.HasExited || _cpuUsageCounter == null || e.Data == null )
         {
            return;
         }

         int framesDone = 0;
         var match = Regex.Match( e.Data, "frame=[ ]*[0-9]+");
         if ( match.Success )
         {
            var numMatch = Regex.Match( match.Groups[0].Value, @"\d+" );
            framesDone = int.Parse( numMatch.Groups[0].Value );
         }

         int cpuUsage = (int)( _cpuUsageCounter.NextValue() / Environment.ProcessorCount );

         EncodingProgress?.Invoke( this, new EncodingProgressEventArgs( framesDone, cpuUsage ) );
      }

      private void CleanupProcessInfo( object sender, EventArgs e )
      {
         if ( _cpuUsageCounter != null )
         {
            _cpuUsageCounter = null;
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
