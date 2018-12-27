using System.IO;
using System.Linq;
using System.Reflection;
using ZemotoCommon.Utils;

namespace Encoder.ffmpeg
{
   internal static class EmbeddedFfmpegManager
   {
      private const string FfmpegFileName = "ffmpeg.exe";
      private const string FfprobeFileName = "ffprobe.exe";

      public static string GetFfmpegExecutableFilePath() => GetExecutableFilePath( FfmpegFileName );
      public static string GetFfprobeExecutableFilePath() => GetExecutableFilePath( FfprobeFileName );

      public static void Cleanup()
      {
         UtilityMethods.SafeDeleteFile( GetFfmpegExecutableFilePath() );
         UtilityMethods.SafeDeleteFile( GetFfprobeExecutableFilePath() );
      }

      private static string GetExecutableFilePath( string executableName )
      {
         if ( !File.Exists( executableName ) )
         {
            ExtractResource( executableName );
         }
         return Path.GetFullPath( executableName );
      }

      private static void ExtractResource( string resourceName )
      {
         using ( var stream = GetResourceStream( resourceName ) )
         {
            using ( var file = File.OpenWrite( resourceName ) )
            {
               stream?.CopyTo( file );
            }
         }
      }

      private static Stream GetResourceStream( string resourceName )
      {
         var assembly = Assembly.GetExecutingAssembly();
         string fullResourceName = assembly.GetManifestResourceNames().First( s => s.EndsWith( resourceName ) );
         return assembly.GetManifestResourceStream( fullResourceName );
      }
   }
}
