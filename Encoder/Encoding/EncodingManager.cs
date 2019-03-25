using System;
using System.Collections.Generic;
using System.Diagnostics;
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

      public void EnqueueAssemblyLines( IEnumerable<AssemblyLine> assemblyLines )
      {
         foreach ( var assemblyLine in assemblyLines )
         {
            assemblyLine.CurrentStepFinished += OnAssemblyLineCurrentStepFinished;
            EnqueueNextStep( assemblyLine );
         }
      }

      private void EnqueueEncodingTasks( IReadOnlyCollection<EncodingTask> tasks )
      {
         if ( !tasks.Any() )
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

      private void EnqueueNextStep( AssemblyLine assemblyLine )
      {
         var nextStep = assemblyLine.GetNextStep();
         if ( nextStep == null )
         {
            assemblyLine.CurrentStepFinished -= OnAssemblyLineCurrentStepFinished;
            return;
         }

         EnqueueEncodingTasks( new List<EncodingTask> { nextStep } );
      }

      private void OnAssemblyLineCurrentStepFinished( object sender, bool success )
      {
         var assemblyLine = (AssemblyLine)sender;
         if ( success )
         {
            EnqueueNextStep( assemblyLine );
         }
         else
         {
            assemblyLine.Cleanup();
            assemblyLine.CurrentStepFinished -= OnAssemblyLineCurrentStepFinished;
         }
      }

      private void CleanupTask( EncodingTask task )
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

      private void DoTask( EncodingTask task )
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

      public void CancelTask( EncodingTask task )
      {
         task.Cancel();

         if ( !task.Started )
         {
            CleanupTask( task );
         }
      }
   }
}
