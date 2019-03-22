using System;
using System.Linq;
using System.Diagnostics;

namespace Encoder.Encoding.Tasks
{
   internal sealed class AssemblyLine : EncodingTaskBase
   {
      private int _stepFinished = -1;
      private EncodingTask _currentStep;
      private readonly EncodingTask[] _steps;
      private readonly string _sourceFile;

      public event EventHandler<bool> CurrentStepFinished;

      public AssemblyLine( string sourceFile, EncodingTask[] steps )
      {
         _sourceFile = sourceFile;
         _steps = steps;
      }

      public EncodingTask GetNextStep()
      {
         Debug.Assert( _currentStep == null );
         if ( _steps.Length <= _stepFinished + 1 )
         {
            return null;
         }

         _currentStep = _steps[_stepFinished + 1];
         _currentStep.TaskFinished += OnStepFinished;

         if ( _stepFinished > 0 )
         {
            var previousStep = _steps[_stepFinished];
            _currentStep.SourceFilePathProvider = previousStep;
         }
         else
         {
            _currentStep.SourceFilePathProvider = this;
         }

         return _currentStep;
      }

      private void OnStepFinished( object sender, bool success )
      {
         _stepFinished++;
         _currentStep = null;
         var step = (EncodingTask)sender;
         step.TaskFinished -= OnStepFinished;
         step.SourceFilePathProvider = null;
         if ( _steps.Length <= _stepFinished )
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
         foreach( var step in _steps.Take( _stepFinished ) )
         {
            step.Cleanup();
         }
         Cancel();
         _steps.ToList().Clear();
      }

      public override void Cancel()
      {
         _currentStep?.Cancel();
      }

      public override string GetFilePath() => _sourceFile;
   }
}
