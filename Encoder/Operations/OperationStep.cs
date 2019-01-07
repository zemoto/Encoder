namespace Encoder.Operations
{
   internal struct OperationStep
   {
      public string Name { get; }
      public string Arguments { get; }

      public OperationStep( string name, string args )
      {
         Name = name;
         Arguments = args;
      }
   }
}
