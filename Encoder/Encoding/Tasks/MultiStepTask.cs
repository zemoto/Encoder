using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZemotoCommon.Utils;

namespace Encoder.Encoding.Tasks
{
   internal struct TaskStep
   {
      public int Step { get; }
      public SingleStepTask Task { get; }

      public TaskStep( int step, SingleStepTask task )
      {
         Step = step;
         Task = task;
      }
   }

   internal sealed class MultiStepTask : EncodingTaskBase
   {
      private int _stepFinished;
      private readonly IEnumerable<TaskStep> _steps;
      private TasksCompletedWatcher _taskWatcher;

      public event EventHandler<bool> CurrentStepFinished;

      public MultiStepTask( IEnumerable<TaskStep> steps )
      {
         _steps = steps;
      }

      public async Task<IEnumerable<SingleStepTask>> GetNextStepAsync()
      {
         var nextStep = _steps.Where( x => x.Step == _stepFinished + 1 ).Select( x => x.Task ).ToList();
         if ( !nextStep.Any() )
         {
            return null;
         }

         bool initialized = true;
         foreach( var task in nextStep )
         {
            if ( !await Task.Run( () => task.Initialize() ) )
            {
               initialized = false;
               break;
            }
         }

         if ( !initialized )
         {
            foreach ( var task in nextStep )
            {
               task.Dispose();
            }
            Cleanup();
            return null;
         }

         _taskWatcher = new TasksCompletedWatcher( nextStep.ToList() );
         _taskWatcher.TasksFinished += OnCurrentStepFinished;
         return nextStep;
      }

      private void OnCurrentStepFinished( object sender, bool success )
      {
         _stepFinished++;
         CurrentStepFinished?.Invoke( this, success );
      }

      public void Cleanup()
      {
         foreach( var step in _steps.Where( x => x.Step <= _stepFinished ) )
         {
            UtilityMethods.SafeDeleteFile( step.Task.TargetFile );
         }
         foreach ( var task in _taskWatcher?.Tasks )
         {
            task.CancelToken.Cancel();
         }
         _steps.ToList().Clear();
      }
   }
}
