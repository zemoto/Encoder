using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJob : IDisposable
   {
      public EncodingJobViewModel Model { get; }

      private readonly CancellationTokenSource _cancelTokenSource;

      private DateTime _startTime;
      private bool _interpolationStarted;
      private FfmpegEncoder _currentEncoder;

      public EncodingJob( List<string> files, int targetFrameRate )
      {
         Model = new EncodingJobViewModel( files, targetFrameRate )
         {
            StopInterpolatingCommand = new RelayCommand( () => _cancelTokenSource.Cancel(), () => _interpolationStarted )
         };

         _cancelTokenSource = new CancellationTokenSource();

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
            Model.IsInterpolating = encodingParams.ShouldInterpolate;
            Model.Progress = 0;

            _currentEncoder = new FfmpegEncoder( encodingParams );
            _currentEncoder.EncodingProgress += OnEncodingProgress;
            _currentEncoder.StartEncoding( _cancelTokenSource.Token );

            Model.CurrentFile = Path.GetFileName( file );
            _startTime = DateTime.Now;

            _currentEncoder.AwaitCompletion();

            Model.SetTimeRemaining( TimeSpan.Zero );
            _currentEncoder.EncodingProgress -= OnEncodingProgress;

            if ( _cancelTokenSource.IsCancellationRequested )
            {
               if ( File.Exists ( newFilename ) )
               {
                  File.Delete( newFilename );
               }
               if ( Directory.GetFiles( targetDir ).Length == 0 )
               {
                  Directory.Delete( targetDir );
               }
               break;
            }
         }
      }

      private void OnEncodingProgress( object sender, EncodingProgressEventArgs e )
      {
         Model.Progress = e.Progress;

         double progress = (int)e.Progress;
         if ( progress == 0 )
         {
            return;
         }

         var ellapsed = DateTime.Now - _startTime;
         var remaining = TimeSpan.FromSeconds( ( (int)ellapsed.TotalSeconds / progress ) * ( 100 - progress ) );

         Model.SetTimeRemaining( remaining );
      }

      public void Dispose()
      {
         _cancelTokenSource?.Dispose();
      }
   }
}
