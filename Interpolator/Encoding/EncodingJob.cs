using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJob : IDisposable
   {
      public EncodingJobViewModel Model { get; }

      private readonly CancellationTokenSource _cancelTokenSource;

      private DateTime _startTime;
      private bool _jobStarted;

      public EncodingJob( List<string> files, int targetFrameRate )
      {
         var tasks = files.Select( x => new EncodingTaskViewModel( x, targetFrameRate ) );

         Model = new EncodingJobViewModel( tasks )
         {
            StopJobCommand = new RelayCommand( () => _cancelTokenSource.Cancel(), () => _jobStarted )
         };

         _cancelTokenSource = new CancellationTokenSource();
      }

      public async Task DoJobAsync()
      {
         _jobStarted = true;

         await InitializeTasks();

         var tasks = new List<Task>
         {
            StartTaskAsync( Model.Tasks.FirstOrDefault() )
         };

         while ( true )
         {
            if ( Model.Tasks.All( x => x.Started ) )
            {
               await Task.WhenAll( tasks );
               break;
            }
            else
            {
               await Task.Delay( 10000 );

               if ( CanSupportMoreTasks() )
               {
                  tasks.Add( StartTaskAsync( Model.Tasks.FirstOrDefault( x => !x.Started ) ) );
               }
            }
         }

         if ( _cancelTokenSource.IsCancellationRequested )
         {
            Model.Tasks.ForEach( x => UtilityMethods.SafeDeleteFile( x.TargetFile ) );
         }
      }

      private async Task InitializeTasks()
      {
         var results = await Task.WhenAll( Model.Tasks.Select( x => x.InitializeTaskAsync() ) );

         var failedTasks = Model.Tasks.Where( x => !results[Model.Tasks.IndexOf( x )] );

         foreach ( var task in failedTasks )
         {
            Model.Tasks.Remove( task );
         }
      }

      private bool CanSupportMoreTasks()
      {
         using ( var cpuUsageMonitor = new ProcessCpuMonitor() )
         {
            var currentTotalCpuUsage = cpuUsageMonitor.GetCurrentCpuUsage();
            var averageTaskCpuUsage = Model.Tasks.Where( x => x.Started ).Average( x => x.CpuUsage );

            return averageTaskCpuUsage < 100 - currentTotalCpuUsage;
         }
      }

      private async Task StartTaskAsync( EncodingTaskViewModel task )
      {
         if ( task == null )
         {
            return;
         }

         var encoder = new FfmpegEncoder( task );
         encoder.EncodingProgress += OnEncodingProgress;
         encoder.StartEncoding( _cancelTokenSource.Token );

         task.FramesDone = 0;
         task.Started = true;
         _startTime = DateTime.Now;

         await Task.Run( () => encoder.AwaitCompletion() );

         task.Finished = !_cancelTokenSource.IsCancellationRequested;
         Model.UpdateJobState( TimeSpan.Zero );
         encoder.EncodingProgress -= OnEncodingProgress;
      }

      private void OnEncodingProgress( object sender, EventArgs e )
      {
         var timeRemaining = TimeSpan.Zero;

         var startedTasks = Model.Tasks.Where( x => x.Started && x.Progress > 0 );
         if ( startedTasks.Any() )
         {
            var averageProgress = startedTasks.Average( x => x.Progress );
            var ellapsed = DateTime.Now - _startTime;
            timeRemaining = TimeSpan.FromSeconds( ( (int)ellapsed.TotalSeconds / averageProgress ) * ( 100 - averageProgress ) );
         }

         Model.UpdateJobState( timeRemaining );
      }

      public void Dispose()
      {
         _cancelTokenSource?.Dispose();
      }
   }
}
