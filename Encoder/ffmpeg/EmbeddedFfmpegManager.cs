using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZemotoCommon.Utils;

namespace Encoder.ffmpeg
{
   internal static class EmbeddedFfmpegManager
   {
      private const string FfmpegFileName = "ffmpeg.exe";
      private const string FfprobeFileName = "ffprobe.exe";

      public static string GetFfmpegExecutableFilePath() => GetExecutableFilePath( FfmpegFileName );
      public static string GetFfprobeExecutableFilePath() => GetExecutableFilePath( FfprobeFileName );

      private static Task _extractingTask;
      public static void BeginExtracting()
      {
         _extractingTask = Task.Run( () =>
         {
            ExtractResource( FfmpegFileName );
            ExtractResource( FfprobeFileName );
         } );
      }

      public static void Cleanup()
      {
         _extractingTask.Wait();
         UtilityMethods.SafeDeleteFile( FfmpegFileName );
         UtilityMethods.SafeDeleteFile( FfprobeFileName );
      }

      private static string GetExecutableFilePath( string executableName )
      {
         _extractingTask.Wait();
         return Path.GetFullPath( executableName );
      }

      private static void ExtractResource( string resourceName )
      {
         var assembly = Assembly.GetExecutingAssembly();
         string fullResourceName = assembly.GetManifestResourceNames().First( s => s.EndsWith( resourceName ) );
         using ( var stream = assembly.GetManifestResourceStream( fullResourceName ) )
         {
            using ( var file = File.OpenWrite( resourceName ) )
            {
               stream?.CopyTo( file );
            }
         }
      }
   }
}
