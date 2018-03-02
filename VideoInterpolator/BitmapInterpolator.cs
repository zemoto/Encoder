using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace VideoInterpolator
{
   internal sealed class BitmapInterpolator
   {
      private readonly byte[] _firstBytes;
      private readonly byte[] _secondBytes;

      private readonly Bitmap _interpolateBitmap;
      private readonly BitmapData _interpolatedBitmapData;

      public BitmapInterpolator( Bitmap first, Bitmap second )
      {
         Debug.Assert( first.Height == second.Height &&
                       first.Width == second.Width &&
                       first.PixelFormat == second.PixelFormat );

         _interpolateBitmap = new Bitmap( first.Width, first.Height, first.PixelFormat );
         _interpolatedBitmapData = _interpolateBitmap.LockBits( new Rectangle( 0, 0, first.Width, first.Height ), ImageLockMode.ReadWrite, first.PixelFormat );

         _firstBytes = GetByteArray( first );
         _secondBytes = GetByteArray( second );
      }

      private void DoInterpolation()
      {
         var interpolatedPixelPtr = _interpolatedBitmapData.Scan0;

         var numBytes = Math.Abs( _interpolatedBitmapData.Stride ) * _interpolateBitmap.Height;
         var interpolatedBytes = new byte[numBytes];

         Marshal.Copy( interpolatedPixelPtr, interpolatedBytes, 0, numBytes );

         for ( int i = 0; i < _firstBytes.Length; i++ )
         {
            interpolatedBytes[i] = InterpolateValues( _firstBytes[i], _secondBytes[i] );
         }

         Marshal.Copy( interpolatedBytes, 0, interpolatedPixelPtr, numBytes );
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

      public Bitmap GetInterpolatedBitmap()
      {
         DoInterpolation();
         _interpolateBitmap.UnlockBits( _interpolatedBitmapData );
         return _interpolateBitmap;
      }
   }
}
