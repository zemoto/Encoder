using System;
using System.Linq;
using System.Collections.Generic;

namespace Encoder.Encoding.Tasks
{
   internal sealed class TasksCompletedWatcher
   {
      public List<SingleStepTask> Tasks { get; }

      public event EventHandler<bool> TasksFinished;

      public TasksCompletedWatcher( IEnumerable<SingleStepTask> tasks )
      {
         Tasks = tasks.ToList();
         foreach( var task in Tasks )
         {
            task.TaskFinished += OnTaskFinished;
         }
      }

      private void OnTaskFinished( object sender, bool success )
      {
         if ( !success )
         {
            TasksFinished?.Invoke( this, false );
            Cleanup();
            return;
         }

         Tasks.Remove( (SingleStepTask)sender );
         if ( Tasks.Count == 0 )
         {
            TasksFinished?.Invoke( this, true );
            Cleanup();
         }
      }

      private void Cleanup()
      {
         foreach ( var task in Tasks )
         {
            task.TaskFinished -= OnTaskFinished;
         }
      }
   }
}
