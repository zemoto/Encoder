using Encoder.Encoding.Tasks;

namespace Encoder.Operations.Separate
{
   internal sealed class SeparateOperation : Operation
   {
      public SeparateOperation()
      {
         AddEncodingOperation( new EncodeWithCustomParams( new EncodingParams( "Separate into video", "-c:v copy -an", "mp4" ) ) );
         AddEncodingOperation( new EncodeWithCustomParams( new EncodingParams( "Separate into audio", "-vn -c:a copy", "wmv" ) ) );
      }
   }
}
