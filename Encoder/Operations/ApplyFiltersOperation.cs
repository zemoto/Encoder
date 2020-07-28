using Encoder.Encoding.Tasks;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Operations
{
   internal sealed class ApplyFiltersOperation : Operation
   {
      public override string Name => $"Apply Filters - V: {_videoFilter.FilterName}, A: {_audioFilter.FilterName}";

      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      public ApplyFiltersOperation( VideoFilter videoFilter, AudioFilter audioFilter )
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
      }

      public override EncodingTask CreateEncodingTask() => new EncodeWithFilters( _videoFilter, _audioFilter );
   }
}
