using Encoder.Operations;

namespace Encoder.Encoding.EncodingTask
{
   internal sealed class EncodeWithOperationStep : EncodingTaskBase
   {
      private readonly OperationStep _operationStep;

      public EncodeWithOperationStep( string sourceFile, OperationStep operationStep ) 
         : base( sourceFile )
      {
         _operationStep = operationStep;
      }

      public override string EncodingArgs => _operationStep.Arguments;
      public override string TargetFileExtension => _operationStep.FileType;
      public override string TaskName => _operationStep.Name;
   }
}
