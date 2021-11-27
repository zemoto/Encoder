using System;
using System.IO;

namespace Encoder.ffmpeg
{
   internal static class FfmpegUtils
   {
      public static string FfmpegFilePath { get; } = Path.GetFullPath( @"ffmpeg\ffmpeg.exe" );
      public static string FfprobeFilePath { get; } = Path.GetFullPath( @"ffmpeg\ffprobe.exe" );

      public static string ToTimeString( this double timeInSeconds ) => TimeSpan.FromSeconds( timeInSeconds ).ToString( @"hh\:mm\:ss\.fff" );
   }
}
