namespace Encoder.Operations
{
   internal struct OperationStep
   {
      public int Step { get; }
      public EncodingParams Params { get; }

      public OperationStep( int step, EncodingParams encodingParams )
      {
         Step = step;
         Params = encodingParams;
      }
   }
}
