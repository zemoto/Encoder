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
      private const string FfmpegFileName = "ffmpeg.exe";

      private static readonly string _ffmpegExeLocation;

      private readonly string _sourceFile;
      private readonly string _targetFile;
      private readonly int _targetFrameRate;

      private Process _currentffmpegProcess = null;

      public event DataReceivedEventHandler OutputReceived;

      static FfmpegEncoder()
      {
         var executingDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
         _ffmpegExeLocation = Path.Combine( executingDir, FfmpegFileName );
      }

      public FfmpegEncoder( string sourceFile, string targetFile, int targetFrameRate )
      {
         _sourceFile = sourceFile;
         _targetFile = targetFile;
         _targetFrameRate = targetFrameRate;
      }

      private string CommonArguments => $"-hide_banner -i \"{_sourceFile}\"";
      private string InterpolationArguments => $"{CommonArguments} -filter:v minterpolate -r {_targetFrameRate} \"{_targetFile}\"";
      private string ReencodeArguments => $"{CommonArguments} -c:v libx264 -crf 18 -preset slow -c:a copy \"{_targetFile}\"";

      public void StartInterpolation( CancellationToken token )
      {
         if ( _currentffmpegProcess != null )
         {
            throw new InvalidOperationException();
         }

         string arguments = ShouldInterpolate() ? InterpolationArguments : ReencodeArguments;

         _currentffmpegProcess = CreateFfmpegProcess( arguments );

         _currentffmpegProcess.ErrorDataReceived += OnErrorDataReceived;
         _currentffmpegProcess.Exited += CleanupProcessInfo;
         _currentffmpegProcess.Start();
         _currentffmpegProcess.BeginErrorReadLine();
         token.Register( () => _currentffmpegProcess?.Kill() );
      }

      private bool ShouldInterpolate()
      {
         bool shouldInterpolate = true;
         var process = CreateFfmpegProcess( CommonArguments );

         process.Start();
         var output = process.StandardError.ReadToEnd();
         var match = Regex.Match( output, "[0-9]* fps" );
         if ( match.Success )
         {
            var currentFrameRate = int.Parse( match.Groups[0].Value.Replace( " fps", "" ) );
            if ( Math.Abs( currentFrameRate - _targetFrameRate ) <= 5 )
            {
               shouldInterpolate = false;
            }
         }

         process.Dispose();
         return shouldInterpolate;
      }

      private void OnErrorDataReceived( object sender, DataReceivedEventArgs e )
      {
         OutputReceived?.Invoke( this, e );
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

      private Process CreateFfmpegProcess( string arguments )
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
