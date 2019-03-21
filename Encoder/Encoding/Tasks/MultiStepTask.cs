using System;
using System.Linq;
using System.Collections.Generic;

namespace Encoder.Encoding.Tasks
{
   internal struct TaskStep
   {
      public int Step { get; }
      public EncodingTaskBase Task { get; }

      public TaskStep( int step, EncodingTaskBase task )
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

      public IEnumerable<EncodingTaskBase> GetNextStep()
      {
         var nextStep = _steps.Where( x => x.Step == _stepFinished + 1 ).Select( x => x.Task ).ToList();
         if ( !nextStep.Any() )
         {
            return null;
         }

         _taskWatcher = new TasksCompletedWatcher( nextStep.ToList() );
         _taskWatcher.TasksFinished += OnCurrentStepFinished;
         return nextStep;
      }

      private void OnCurrentStepFinished( object sender, bool success )
      {
         _stepFinished++;
         if ( _stepFinished >= _steps.Select( x => x.Step ).Max() )
         {
            RaiseTaskFinished( success );
         }
         else
         {
            CurrentStepFinished?.Invoke( this, success );
         }
      }

      public override void Cleanup()
      {
         foreach( var step in _steps.Where( x => x.Step <= _stepFinished ) )
         {
            step.Task.Cleanup();
         }
         Cancel();
         _steps.ToList().Clear();
      }

      public override void Cancel()
      {
         foreach ( var task in _taskWatcher?.Tasks )
         {
            task.Cancel();
         }
      }
   }
}
