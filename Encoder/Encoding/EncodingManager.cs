using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Encoder.Encoding.Tasks;
using ZemotoUtils;
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

      public void EnqueueTasks( IReadOnlyCollection<EncodingTaskBase> tasks )
      {
         if ( tasks.Count == 0 )
         {
            return;
         }

         foreach ( var task in tasks )
         {
            Application.Current.Dispatcher.Invoke( () => Model.Tasks.Add( task ) );
         }

         if ( Model.NoTasksStarted )
         {
            StartNextTask();
         }

         _taskStartTimer.Start();
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
         _ = Task.Run( () => DoTask( Model.NextPendingTask ) );
      }

      private void DoTask( EncodingTaskBase task )
      {
         if ( task?.Started != false )
         {
            return;
         }

         bool success = task.DoWork();
         if ( !success )
         {
            task.Cleanup();
         }

         if ( !string.IsNullOrEmpty( task.Error ) )
         {
            _ = MessageBox.Show( $"Error: {task.Error}", task.SourceFile, MessageBoxButton.OK, MessageBoxImage.Error );
         }

         DisposeTask( task );
      }

      public void CancelTask( EncodingTaskBase task )
      {
         task.Cancel();
         _ = Application.Current.Dispatcher.Invoke( () => Model.Tasks.Remove( task ) );

         if ( !task.Started )
         {
            DisposeTask( task );
         }
      }

      private void DisposeTask( EncodingTaskBase task )
      {
         bool taskWasStarted = task.Started;

         task.Dispose();
         _ = Application.Current.Dispatcher.Invoke( () => Model.Tasks.Remove( task ) );

         if ( !Model.AnyTasksPending )
         {
            _taskStartTimer.Stop();
         }

         if ( taskWasStarted )
         {
            CheckIfCanStartNewTask();
         }
      }
   }
}
