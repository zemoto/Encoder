namespace Encoder.Operations.Separate
{
   internal sealed class SeparateOperation : Operation
   {
      public SeparateOperation()
      {
         Steps.Add( new OperationStep( "Separate into video", "-c:v copy -an", "mp4" )  );
         Steps.Add( new OperationStep( "Separate into audio", "-vn -c:a copy", "wmv" )  );
      }
   }
}
