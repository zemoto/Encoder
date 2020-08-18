namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithCustomParams : EncodingTask
   {
      private readonly string _targetExtension;

      public EncodeWithCustomParams( string args, string targetExtension )
      {
         EncodingArgs = args;
         _targetExtension = targetExtension;
      }

      protected override void InitializeEx()
      {
         if ( !string.IsNullOrEmpty( _targetExtension ) )
         {
            TargetFileExtension = _targetExtension;
         }

         // Can't be sure how the target will change. This being 0 will cause the UI to not try to track progress.
         TargetTotalFrames = 0;
      }

      public override string EncodingArgs { get; }
      public override string TaskName => EncodingArgs;
      public override string DetailedTaskName => $"Custom - \"{TaskName}\"";
   }
}
