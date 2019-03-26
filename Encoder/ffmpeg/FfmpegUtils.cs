using System;
using System.Collections.Generic;

namespace Encoder.ffmpeg
{
   internal static class FfmpegUtils
   {
      public static string ToTimeString( this double timeInSeconds ) => TimeSpan.FromSeconds( timeInSeconds ).ToString( @"hh\:mm\:ss\.fff" );
   }
}
