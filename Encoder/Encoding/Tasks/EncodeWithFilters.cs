using Encoder.Filters;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithFilters : EncodingTask
   {
      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      public EncodeWithFilters( VideoFilter videoFilter, AudioFilter audioFilter ) 
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
      }

      public override bool Initialize( string directory, int id = -1 )
      {
         if ( !base.Initialize( directory, id ) )
         {
            return false;
         }
         
         _videoFilter.Initialize( SourceFrameRate, SourceDuration );
         TargetTotalFrames = _videoFilter.TargetTotalFrames;
         TargetBitrate = (int)( TargetBitrate * _videoFilter.BitRateMultipler );
         return true;
      }

      public override string EncodingArgs => $"{FilterArgumentBuilder.GetFilterArguments( _videoFilter )} {FilterArgumentBuilder.GetFilterArguments( _audioFilter )}";

      public override string TaskName => _videoFilter.FilterName;
   }
}
