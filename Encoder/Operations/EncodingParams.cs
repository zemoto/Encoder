namespace Encoder.Operations
{
   internal readonly struct EncodingParams
   {
      public string Name { get; }
      public string Arguments { get; }
      public string FileType { get; }
      public bool DurationChanging { get; }

      public EncodingParams( string name, string args, string fileType = null, bool durationChanging = false )
      {
         Name = name;
         Arguments = args;
         FileType = fileType;
         DurationChanging = durationChanging;
      }
   }
}
