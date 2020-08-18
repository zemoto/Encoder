using System;
using System.Linq;

namespace Encoder.Encoding
{
   internal sealed class VideoMetadata
   {
      public double FrameRate { get; }
      public TimeSpan Duration { get; }
      public uint Bitrate { get; }

      public VideoMetadata( double frameRate, TimeSpan duration, uint bitRate )
      {
         FrameRate = frameRate;
         Duration = duration;
         Bitrate = bitRate;
      }
   }

   internal sealed class CropDetect
   {
      private const string CropDetectFlag = "[Parsed_cropdetect";
      private const string CropFlag = "crop=";

      public int Width { get; }
      public int Height { get; }
      public int X { get; }
      public int Y { get; }

      private CropDetect( int width, int height, int x, int y )
      {
         Width = width;
         Height = height;
         X = x;
         Y = y;
      }

      public static CropDetect CreateFromString( string value )
      {
         var cropDetectData = value.Replace( "\r", string.Empty ).Split( '\n' ).Where( x => x.StartsWith( CropDetectFlag ) ).FirstOrDefault();
         if ( !string.IsNullOrEmpty( cropDetectData ) )
         {
            var cropData = cropDetectData.Split( ' ' ).Where( x => x.StartsWith( CropFlag ) ).FirstOrDefault();
            if ( !string.IsNullOrEmpty( cropData ) )
            {
               var parameters = cropData.Replace( CropFlag, string.Empty ).Split( ':' );
               if ( parameters.Count() == 4 )
               {
                  int width = int.Parse( parameters[0] );
                  int height = int.Parse( parameters[1] );
                  int x = int.Parse( parameters[2] );
                  int y = int.Parse( parameters[3] );

                  return new CropDetect( width, height, x, y );
               }
            }
         }

         return null;
      }

      public static CropDetect SelectSmallestCrop( CropDetect first, CropDetect second )
      {
         int width = Math.Min( first.Width, second.Width );
         int height = Math.Min( first.Height, second.Height );
         int x = Math.Min( first.X, second.X );
         int y = Math.Min( first.Y, second.Y );

         return new CropDetect( width, height, x, y );
      }
   }
}