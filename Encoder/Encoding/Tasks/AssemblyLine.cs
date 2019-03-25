using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using ZemotoCommon.Utils;

namespace Encoder.Encoding.Tasks
{
   internal sealed class AssemblyLine : EncodingTaskBase
   {
      private int _stepFinished = -1;
      private EncodingTask _currentStep;
      private readonly EncodingTask[] _steps;
      private readonly string _sourceFile;
      private readonly string _assemblyLineDirectory;

      private readonly string _assemblyLineId = Guid.NewGuid().ToString( "N" ).Substring( 0, 8 );

      public event EventHandler<bool> CurrentStepFinished;

      public AssemblyLine( string sourceFile, EncodingTask[] steps )
      {
         _sourceFile = sourceFile;
         _steps = steps;

         _assemblyLineDirectory = Path.Combine( Path.GetDirectoryName( _sourceFile ), "done", _assemblyLineId );
      }

      public EncodingTask GetNextStep()
      {
         Debug.Assert( _currentStep == null );
         if ( _steps.Length <= _stepFinished + 1 )
         {
            return null;
         }

         _currentStep = _steps[_stepFinished + 1];

         if ( _stepFinished >= 0 )
         {
            var previousStep = _steps[_stepFinished];
            _currentStep.SourceFilePathProvider = previousStep;
         }
         else
         {
            _currentStep.SourceFilePathProvider = this;
         }

         UtilityMethods.CreateDirectory( _assemblyLineDirectory );

         if ( !_currentStep.Initialize( _assemblyLineDirectory, _stepFinished + 1 ) )
         {
            _currentStep.SourceFilePathProvider = null;
            return null;
         }
         
         _currentStep.TaskFinished += OnStepFinished;
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
         Cancel();

         foreach( var step in _steps.Take( _stepFinished ) )
         {
            step.Cleanup();
         }

         UtilityMethods.SafeDeleteDirectory( _assemblyLineDirectory );
      }

      public override void Cancel()
      {
         _currentStep?.Cancel();
      }

      public override string GetFilePath() => _sourceFile;
   }
}
