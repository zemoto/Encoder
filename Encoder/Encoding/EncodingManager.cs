using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using ZemotoCommon.Utils;
using Timer = System.Timers.Timer;

namespace Encoder.Encoding
{
   internal sealed class EncodingManager : IDisposable
   {
      public EncodingManagerViewModel Model { get; } = new EncodingManagerViewModel();

      private readonly Timer _taskStartTimer;

      public EncodingManager()
      {
         _taskStartTimer = new Timer( 30000 );
         _taskStartTimer.Elapsed += OnTaskStartTimerTick;
      }

      public async Task EnqueueTasksAsync( IEnumerable<EncodingTaskViewModel> tasks )
      {
         foreach( var task in tasks )
         {
            if ( !await Task.Run( () => task.Initialize() ) )
            {
               task.Dispose();
               continue;
            }

            Model.Tasks.Add( task );
         }

         if ( Model.NoTasksStarted )
         {
            StartNextTask();
         }

         _taskStartTimer.Start();
      }

      private void CleanupTask( EncodingTaskViewModel task )
      {
         bool taskWasStarted = task.Started;

         task.Dispose();
         Application.Current.Dispatcher.Invoke( () => Model.Tasks.Remove( task ) );

         if ( !Model.AnyTasksPending )
         {
            _taskStartTimer.Stop();
         }

         if ( taskWasStarted )
         {
            CheckIfCanStartNewTask();
         }
      }

      public void Dispose()
      {
         _taskStartTimer.Dispose();
      }

      private void OnTaskStartTimerTick( object sender, ElapsedEventArgs e )
      {
         if ( Model.AllTasksStarted )
         {
            _taskStartTimer.Stop();
         }
         else
         {
            CheckIfCanStartNewTask();
         }
      }

      private void CheckIfCanStartNewTask()
      {
         if ( CanSupportMoreTasks() )
         {
            StartNextTask();
         }
      }

      private bool CanSupportMoreTasks()
      {
         if ( !Model.AnyTasksPending )
         {
            return false;
         }

         if ( Model.NoTasksStarted )
         {
            return true;
         }

         var currentTotalCpuUsage = ProcessCpuMonitor.GetTotalCpuUsage();
         var averageTaskCpuUsage = Model.Tasks.Where( x => x.Started ).Average( x => x.CpuUsage );

         return averageTaskCpuUsage < 100 - currentTotalCpuUsage;
      }

      private void StartNextTask()
      {
         Task.Run( () => DoTask( Model.NextPendingTask ) );
      }

      private void DoTask( EncodingTaskViewModel task )
      {
         if ( task == null )
         {
            return;
         }

         if ( task.CancelToken.IsCancellationRequested )
         {
            CleanupTask( task );
            return;
         }

         var encoder = new FfmpegEncoder( task );
         encoder.StartEncoding( task.CancelToken.Token );

         encoder.AwaitCompletion();

         task.Finished = !task.CancelToken.IsCancellationRequested;

         if ( !task.Finished )
         {
            Task.Delay( 300 );
            UtilityMethods.SafeDeleteFile( task.TargetFile );
         }

         CleanupTask( task );
      }

      public void CancelTask( EncodingTaskViewModel task )
      {
         task.CancelToken.Cancel();

         if ( !task.Started )
         {
            CleanupTask( task );
         }
      }
   }
}
