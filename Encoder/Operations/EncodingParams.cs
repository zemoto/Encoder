namespace Encoder.Operations
{
   internal readonly struct EncodingParams
   {
      public string Name { get; }
      public string Arguments { get; }
      public string FileType { get; }

      public EncodingParams( string name, string args, string fileType )
      {
         Name = name;
         Arguments = args;
         FileType = fileType.Trim( '.' );
      }
   }
}
