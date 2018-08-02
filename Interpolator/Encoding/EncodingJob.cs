using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Interpolator.Utils;
using Timer = System.Timers.Timer;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJob : IDisposable
   {
      public EncodingJobViewModel Model { get; }

      private readonly CancellationTokenSource _cancelTokenSource;

      private DateTime _startTime = DateTime.MinValue;
      private Timer _jobRefreshTimer;

      public EncodingJob( List<string> files, int targetFrameRate )
      {
         var tasks = files.Select( x => new EncodingTaskViewModel( x, targetFrameRate ) );

         Model = new EncodingJobViewModel( tasks )
         {
            StopJobCommand = new RelayCommand( () => _cancelTokenSource.Cancel(), () => _startTime != DateTime.MinValue )
         };

         _cancelTokenSource = new CancellationTokenSource();

         _jobRefreshTimer = new Timer( 3000 );
         _jobRefreshTimer.Elapsed += OnRefreshTimerTick;
      }

      private void OnRefreshTimerTick( object sender, ElapsedEventArgs e )
      {
         var totalFrames = Model.Tasks.Select( x => x.TargetTotalFrames ).Aggregate( (x,y) => x += y );
         var finishedFrames = Model.Tasks.Select( x => x.FramesDone ).Aggregate( (x,y) => x += y );

         var timeRemaining = TimeSpan.Zero;
         if ( finishedFrames > 0 )
         {
            var ellapsed = DateTime.Now - _startTime;
            timeRemaining = TimeSpan.FromSeconds( ( ellapsed.TotalSeconds / finishedFrames ) * ( totalFrames - finishedFrames ) );
         }

         Model.UpdateJobState( timeRemaining );
      }

      public async Task DoJobAsync()
      {
         await InitializeTasks();

         _startTime = DateTime.Now;
         var tasks = new List<Task>
         {
            StartTaskAsync( Model.Tasks.FirstOrDefault() )
         };

         _jobRefreshTimer.Start();

         while ( true )
         {
            if ( _cancelTokenSource.IsCancellationRequested || Model.Tasks.All( x => x.Started ) )
            {
               await Task.WhenAll( tasks );
               break;
            }
            else
            {
               await Task.Delay( 30000, _cancelTokenSource.Token ).ContinueWith( x => { } );
               if ( !_cancelTokenSource.IsCancellationRequested && CanSupportMoreTasks() )
               {
                  tasks.Add( StartTaskAsync( Model.Tasks.FirstOrDefault( x => !x.Started ) ) );
               }
            }
         }

         _jobRefreshTimer.Stop();

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
         encoder.StartEncoding( _cancelTokenSource.Token );

         await Task.Run( () => encoder.AwaitCompletion() );

         task.Finished = !_cancelTokenSource.IsCancellationRequested;
      }

      public void Dispose()
      {
         _cancelTokenSource?.Dispose();
         _jobRefreshTimer?.Dispose();
      }
   }
}
