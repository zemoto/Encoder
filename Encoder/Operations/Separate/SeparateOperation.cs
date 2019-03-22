namespace Encoder.Operations.Separate
{
   internal sealed class SeparateOperation : Operation
   {
      public SeparateOperation()
      {
         AddEncodingTask( new EncodingParams( "Separate into video", "-c:v copy -an", "mp4" ), true );
         AddEncodingTask( new EncodingParams( "Separate into audio", "-vn -c:a copy", "wmv" ), true );
      }
   }
}
