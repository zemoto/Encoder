using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace VideoInterpolator
{
   internal static class BitmapInterpolator
   {
      public static Bitmap InterpolateBitmaps( Bitmap first, Bitmap second )
      {
         var interpolateBitmap = new Bitmap( first.Width, first.Height, first.PixelFormat );
         var interpolatedBitmapData = interpolateBitmap.LockBits( new Rectangle( 0, 0, first.Width, first.Height ), ImageLockMode.ReadWrite, first.PixelFormat );

         var firstBytes = GetByteArray( first );
         var secondBytes = GetByteArray( second );

         var numBytes = Math.Abs( interpolatedBitmapData.Stride ) * interpolateBitmap.Height;
         var interpolatedBytes = new byte[numBytes];

         Marshal.Copy( interpolatedBitmapData.Scan0, interpolatedBytes, 0, numBytes );

         for ( int i = 0; i < firstBytes.Length; i++ )
         {
            interpolatedBytes[i] = InterpolateValues( firstBytes[i], secondBytes[i] );
         }

         Marshal.Copy( interpolatedBytes, 0, interpolatedBitmapData.Scan0, numBytes );
         interpolateBitmap.UnlockBits( interpolatedBitmapData );

         return interpolateBitmap;
      }

      private static byte InterpolateValues( byte first, byte second )
      {
         return (byte)( ( first + second ) / 2 );
      }

      private static byte[] GetByteArray( Bitmap bitmap )
      {
         var data = bitmap.LockBits( new Rectangle( 0, 0, bitmap.Width, bitmap.Height ), ImageLockMode.ReadWrite, bitmap.PixelFormat );

         int numBytes = Math.Abs( data.Stride ) * data.Height;
         var bytes = new byte[numBytes];

         Marshal.Copy( data.Scan0, bytes, 0, numBytes );
         bitmap.UnlockBits( data );

         return bytes;
      }
   }
}
