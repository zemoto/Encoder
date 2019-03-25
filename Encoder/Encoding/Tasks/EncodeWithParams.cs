using System;
using Encoder.Operations;

namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithCustomParams : EncodingTask
   {
      private readonly EncodingParams _encodingParams;

      public EncodeWithCustomParams( EncodingParams encodingParams )
      {
         _encodingParams = encodingParams;
      }

      public override bool Initialize( string directory, int id )
      {
         if ( base.Initialize( directory, id ) )
         {
            if ( _encodingParams.DurationChanging )
            {
               SourceDuration = TimeSpan.Zero;
               SourceFrameRate = 0;
               TargetTotalFrames = 0;
            }

            return true;
         }

         return false;
      }

      public override string EncodingArgs => _encodingParams.Arguments;
      public override string TargetFileExtension => _encodingParams.FileType;
      public override string TaskName => _encodingParams.Name;
   }
}
