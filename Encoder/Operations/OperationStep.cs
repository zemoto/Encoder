namespace Encoder.Operations
{
   internal struct OperationStep
   {
      public string Name { get; }
      public string Arguments { get; }
      public string FileType { get; }

      public OperationStep( string name, string args, string fileType )
      {
         Name = name;
         Arguments = args;
         FileType = fileType;
      }
   }
}
