namespace Encoder.Encoding.Tasks
{
   internal interface IFilePathProvider
   {
      string GetFilePath();
   }

   internal sealed class FilePathProvider : IFilePathProvider
   {
      private readonly string _filePath;
      public FilePathProvider( string filePath )
      {
         _filePath = filePath;
      }

      public string GetFilePath() => _filePath;
   }
}
