using Encoder.Filters;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Encoding.EncodingTask
{
   internal sealed class EncodeWithFiltersTask : EncodingTaskBase
   {
      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      public EncodeWithFiltersTask( string sourceFile, VideoFilter videoFilter, AudioFilter audioFilter ) 
         : base( sourceFile )
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
      }

      public override string GetEncodingArgs() =>
         $"{FilterArgumentBuilder.GetFilterArguments( _videoFilter )} {FilterArgumentBuilder.GetFilterArguments( _audioFilter )} \"{TargetFile}\"";

      public override bool Initialize()
      {
         if ( base.Initialize() )
         {
            _videoFilter.Initialize( SourceFrameRate, SourceDuration );
            TargetTotalFrames = _videoFilter.TargetTotalFrames;
            return true;
         }

         return false;
      }

      public override string TaskName => _videoFilter.FilterName;
   }
}
