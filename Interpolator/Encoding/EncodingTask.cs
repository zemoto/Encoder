using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Interpolator.Encoding
{
   internal sealed class EncodingTask : IDisposable
   {
      public EncodingTaskViewModel Model { get; }

      private readonly Timer _interpolationTimer;
      private readonly CancellationTokenSource _cancelTokenSource;

      private DateTime _startTime;
      private bool _interpolationStarted;
      private FfmpegEncoder _currentEncoder;

      public EncodingTask( List<string> files, int targetFrameRate )
      {
         Model = new EncodingTaskViewModel( files, targetFrameRate )
         {
            StopInterpolatingCommand = new RelayCommand( () => _cancelTokenSource.Cancel(), () => _interpolationStarted )
         };

         _cancelTokenSource = new CancellationTokenSource();

         _interpolationTimer = new Timer( 1000 );
         _interpolationTimer.Elapsed += ( s, a ) => UpdateEllapsedTime();
      }

      private void UpdateEllapsedTime()
      {
         Model.CurrentFileEllapsedEncodingTime = _interpolationTimer.Enabled ? ( DateTime.Now - _startTime ).ToString( @"hh\:mm\:ss" ) : "00:00:00";
      }

      public void StartEncoding()
      {
         _interpolationStarted = true;
         var files = new List<string>( Model.Files );

         foreach ( var file in files )
         {
            var targetDir = Path.Combine( Path.GetDirectoryName( file ), "interpolated" );
            if ( !Directory.Exists( targetDir ) )
            {
               Directory.CreateDirectory( targetDir );
            }

            var newFilename = Path.Combine( targetDir, Path.GetFileNameWithoutExtension( file ) + ".mp4" );

            _currentEncoder = new FfmpegEncoder( file, newFilename, Model.TargetFrameRate );
            _currentEncoder.OutputReceived += OnOutputReceived;
            _currentEncoder.StartInterpolation( _cancelTokenSource.Token );

            Model.CurrentFile = Path.GetFileName( file );
            _startTime = DateTime.Now;
            _interpolationTimer.Start();

            _currentEncoder.AwaitCompletion();

            _interpolationTimer.Stop();
            UpdateEllapsedTime();
            _currentEncoder.OutputReceived -= OnOutputReceived;

            if ( _cancelTokenSource.IsCancellationRequested )
            {
               File.Delete( newFilename );
               if ( Directory.GetFiles( targetDir ).Length == 0 )
               {
                  Directory.Delete( targetDir );
               }
               break;
            }
         }
      }

      private void OnOutputReceived( object sender, DataReceivedEventArgs e )
      {
         Model.EncoderOutput = e.Data;
      }

      public void Dispose()
      {
         _interpolationTimer?.Dispose();
         _cancelTokenSource?.Dispose();
      }
   }
}
