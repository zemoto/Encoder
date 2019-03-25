using System.Collections.Generic;
using Encoder.Encoding.Tasks;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Operations
{
   internal sealed class ApplyFiltersOperation : Operation
   {
      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      public ApplyFiltersOperation( VideoFilter videoFilter, AudioFilter audioFilter )
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
      }

      public override List<EncodingTask[]> CreateOperationChains( string file )
      {
         return new List<EncodingTask[]>
         {
            new EncodingTask[] {new EncodeWithFilters( _videoFilter, _audioFilter )}
         };
      }
   }
}
