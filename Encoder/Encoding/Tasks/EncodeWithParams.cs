using Encoder.Operations;

namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithCustomParams : SingleStepTask
   {
      public EncodeWithCustomParams( string sourceFile, EncodingParams encodingParams ) 
         : base( sourceFile )
      {
         EncodingArgs = encodingParams.Arguments;
         TargetFileExtension = encodingParams.FileType;
         TaskName = encodingParams.Name;
      }

      public override string EncodingArgs { get; }
      public override string TargetFileExtension { get; }
      public override string TaskName { get; }
   }
}
