using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Encoder.Encoding.Tasks;

namespace Encoder.Encoding
{
   internal sealed class EncodingManager
   {
      public EncodingManagerViewModel Model { get; } = new EncodingManagerViewModel();

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

         if ( taskWasStarted )
         {
            StartNextTask();
         }
      }
   }
}
