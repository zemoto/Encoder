namespace Encoder.Operations.Separate
{
   internal sealed class SeparateOperation : Operation
   {
      public SeparateOperation()
      {
         Steps.Add( new OperationStep( "Separate into video", "-c:v copy -an {0}.mp4" )  );
         Steps.Add( new OperationStep( "Separate into audio", "-vn -c:a copy {0}.mp3" )  );
      }
   }
}
