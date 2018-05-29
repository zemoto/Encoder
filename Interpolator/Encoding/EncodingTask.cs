using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
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

      public void Start()
      {
         _interpolationStarted = true;
         var files = new List<string>( Model.Files );

         foreach ( var file in files )
         {
            if ( !FfmpegEncoder.GetVideoInfo( file, out double frameRate, out TimeSpan duration ) )
            {
               MessageBox.Show( $"Could not read video file: {file}" );
               continue;
            }

            var targetDir = Path.Combine( Path.GetDirectoryName( file ), "interpolated" );
            if ( !Directory.Exists( targetDir ) )
            {
               Directory.CreateDirectory( targetDir );
            }

            var newFilename = Path.Combine( targetDir, Path.GetFileNameWithoutExtension( file ) + ".mp4" );

            var encodingParams = new EncodingParameters( file, frameRate, duration, newFilename, Model.TargetFrameRate );
            _currentEncoder = new FfmpegEncoder( encodingParams );
            _currentEncoder.OutputReceived += OnOutputReceived;
            _currentEncoder.StartEncoding( _cancelTokenSource.Token );

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
