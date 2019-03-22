using Encoder.Encoding.Tasks;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Operations.ApplyFilters
{
   internal sealed class ApplyFiltersOperation : Operation
   {
      public ApplyFiltersOperation( VideoFilter videoFilter, AudioFilter audioFilter )
      {
         AddEncodingOperation( new EncodeWithFilters( videoFilter, audioFilter ) );
      }
   }
}
