using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Encoder.Utils;
using Timer = System.Timers.Timer;

namespace Encoder.Encoding
{
   internal sealed class EncodingManager : IDisposable
   {
      public EncodingManagerViewModel Model { get; } = new EncodingManagerViewModel();

      private DateTime _startTime = DateTime.MinValue;
      private readonly Timer _refreshTimer;
      private readonly Timer _taskStartTimer;

      public EncodingManager()
      {
         _refreshTimer = new Timer( 3000 );
         _refreshTimer.Elapsed += OnRefreshTimerTick;

         _taskStartTimer = new Timer( 30000 );
         _taskStartTimer.Elapsed += OnTaskStartTimerTick;

         Model.Tasks.CollectionChanged += OnTasksCollectionChanged;
      }

      private void OnTasksCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
      {
         if ( e.Action == NotifyCollectionChangedAction.Add )
         {
            if ( !Model.Tasks.Any( x => x.Started ) )
            {
               _startTime = DateTime.Now;
               StartNextTask();
            }
            _refreshTimer.Start();
            _taskStartTimer.Start();
         }
         else if ( e.Action == NotifyCollectionChangedAction.Remove && !Model.Tasks.Any() )
         {
            _refreshTimer.Stop();
            _taskStartTimer.Stop();
         }
      }

      public void Dispose()
      {
         _refreshTimer.Dispose();
         _taskStartTimer.Dispose();
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

         Model.UpdateState( timeRemaining );
      }

      private void OnTaskStartTimerTick( object sender, ElapsedEventArgs e )
      {
         if ( Model.Tasks.All( x => x.Started ) )
         {
            _taskStartTimer.Stop();
         }
         else if ( CanSupportMoreTasks() )
         {
            StartNextTask();
         }
      }

      private bool CanSupportMoreTasks()
      {
         var currentTotalCpuUsage = TotalCpuMonitor.GetCurrentCpuUsage();
         var averageTaskCpuUsage = Model.Tasks.Where( x => x.Started ).Average( x => x.CpuUsage );

         return averageTaskCpuUsage < 100 - currentTotalCpuUsage;
      }

      private void StartNextTask()
      {
         Task.Run( () => DoTask( Model.Tasks.FirstOrDefault( x => !x.Started ) ) );
      }

      private void DoTask( EncodingTaskViewModel task )
      {
         if ( task == null )
         {
            return;
         }

         if ( task.Initialized || task.Initialize() )
         {
            var encoder = new FfmpegEncoder( task );
            encoder.StartEncoding( task.CancelToken.Token );

            encoder.AwaitCompletion();

            task.Finished = !task.CancelToken.IsCancellationRequested;

            if ( !task.Finished )
            {
               UtilityMethods.SafeDeleteFile( task.TargetFile );
            }
         }

         task.Dispose();
         Application.Current.Dispatcher.Invoke( () => Model.Tasks.Remove( task ) ); 
      }
   }
}
