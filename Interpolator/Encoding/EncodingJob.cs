using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJob : IDisposable
   {
      public EncodingJobViewModel Model { get; }

      private readonly CancellationTokenSource _cancelTokenSource;

      private DateTime _startTime;
      private bool _jobStarted;
      private FfmpegEncoder _currentEncoder;

      public EncodingJob( List<string> files, int targetFrameRate )
      {
         var failedFiles = new List<string>();
         var tasks = new List<EncodingTaskViewModel>();
         foreach( var file in files )
         {
            try
            {
               var task = new EncodingTaskViewModel( file, targetFrameRate );
               tasks.Add( task );
            }
            catch
            {
               failedFiles.Add( file );
            }
         }
         if ( failedFiles.Any() )
         {
            MessageBox.Show( $"Could not read video file(s): {Environment.NewLine + failedFiles.Aggregate( ( x, y ) => x + Environment.NewLine + y )}" );
         }

         Model = new EncodingJobViewModel( tasks )
         {
            StopJobCommand = new RelayCommand( () => _cancelTokenSource.Cancel(), () => _jobStarted )
         };

         _cancelTokenSource = new CancellationTokenSource();
      }

      public void DoJob()
      {
         _jobStarted = true;
         var tasks = new List<EncodingTaskViewModel>( Model.Tasks );

         foreach ( var task in tasks )
         {
            var targetDir = Path.GetDirectoryName( task.TargetFile );
            if ( !Directory.Exists( targetDir ) )
            {
               Directory.CreateDirectory( targetDir );
            }

            _currentEncoder = new FfmpegEncoder( task );
            _currentEncoder.EncodingProgress += OnEncodingProgress;
            _currentEncoder.StartEncoding( _cancelTokenSource.Token );

            task.Progress = 0;
            task.Started = true;
            Model.CurrentTask = task;
            _startTime = DateTime.Now;

            _currentEncoder.AwaitCompletion();

            task.Progress = 100;
            Model.SetTimeRemaining( TimeSpan.Zero );
            _currentEncoder.EncodingProgress -= OnEncodingProgress;

            if ( _cancelTokenSource.IsCancellationRequested )
            {
               if ( File.Exists ( task.TargetFile ) )
               {
                  File.Delete( task.TargetFile );
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
         Model.CurrentTask.Progress = e.Progress;

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
