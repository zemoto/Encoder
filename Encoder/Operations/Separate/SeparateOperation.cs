namespace Encoder.Operations.Separate
{
   internal sealed class SeparateOperation : Operation
   {
      public SeparateOperation()
      {
         Steps.Add( new OperationStep( 1, new EncodingParams( "Separate into video", "-c:v copy -an", "mp4" ) ) );
         Steps.Add( new OperationStep( 1, new EncodingParams( "Separate into audio", "-vn -c:a copy", "wmv" ) ) );
      }
   }
}
