using System;
using System.Collections.Generic;
using System.Linq;

namespace Encoder.ffmpeg
{
   internal static class FfmpegUtils
   {
      public static string ToTimeString( this double timeInSeconds ) => TimeSpan.FromSeconds( timeInSeconds ).ToString( @"hh\:mm\:ss\.ffffff" );

      // This method assumes keyframes are sorted
      public static double GetClosestKeyFrame( double target, IEnumerable<double> keyFrames )
      {
         var closest = 0.0;
         var distance = double.PositiveInfinity;
         foreach ( var keyFrame in keyFrames.Where( x => x >= target ) )
         {
            var newDistance = Math.Abs( target - keyFrame );
            if ( newDistance < distance )
            {
               distance = newDistance;
               closest = keyFrame;
            }
            else
            {
               break;
            }
         }

         return closest;
      }
   }
}
