using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Encoder.Encoding.Tasks;
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

      public async Task EnqueueTasksAsync( List<EncodingTaskBase> tasks )
      {
         await EnqueueMultiStepTasksAsync( tasks.OfType<MultiStepTask>() );
         await EnqueueSingleStepTasksAsync( tasks.OfType<SingleStepTask>().ToList(), true );
      }

      private async Task EnqueueSingleStepTasksAsync( IReadOnlyCollection<SingleStepTask> tasks, bool initializeTasks )
      {
         if ( !tasks.Any() )
         {
            return;
         }

         foreach ( var task in tasks )
         {
            if ( initializeTasks && !await Task.Run( () => task.Initialize() ) )
            {
               task.Dispose();
               continue;
            }

            Application.Current.Dispatcher.Invoke( () => Model.Tasks.Add( task ) );
         }

         if ( Model.NoTasksStarted )
         {
            StartNextTask();
         }

         _taskStartTimer.Start();
      }

      private async Task EnqueueMultiStepTasksAsync( IEnumerable<MultiStepTask> multiStepTasks )
      {
         foreach ( var multiStepTask in multiStepTasks )
         {
            multiStepTask.CurrentStepFinished += OnMultiStepTaskCurrentStepFinished;
            await EnqueueNextStep( multiStepTask );
         }
      }

      private async Task EnqueueNextStep( MultiStepTask multiStepTask )
      {
         var tasks = await multiStepTask.GetNextStepAsync();
         if ( tasks == null )
         {
            multiStepTask.CurrentStepFinished -= OnMultiStepTaskCurrentStepFinished;
            return;
         }

         await EnqueueSingleStepTasksAsync( tasks.ToList(), false );
      }

      private async void OnMultiStepTaskCurrentStepFinished( object sender, bool success )
      {
         var multiStepTask = (MultiStepTask)sender;
         if ( success )
         {
            await EnqueueNextStep( multiStepTask );
         }
         else
         {
            multiStepTask.Cleanup();
            multiStepTask.CurrentStepFinished -= OnMultiStepTaskCurrentStepFinished;
         }
      }

      private void CleanupTask( SingleStepTask task )
      {
         bool taskWasStarted = task.Started;
         task.SetTaskFinished();

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

      private void DoTask( SingleStepTask task )
      {
         if ( task == null || task.Started )
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

         if ( task.CancelToken.IsCancellationRequested )
         {
            Task.Delay( 300 );
            UtilityMethods.SafeDeleteFile( task.TargetFile );
         }

         if ( !string.IsNullOrEmpty( encoder.Error ) )
         {
            MessageBox.Show( $"Error: {encoder.Error}", task.SourceFile, MessageBoxButton.OK, MessageBoxImage.Error );
         }

         CleanupTask( task );
      }

      public void CancelTask( SingleStepTask task )
      {
         task.CancelToken.Cancel();

         if ( !task.Started )
         {
            CleanupTask( task );
         }
      }
   }
}
