using System;

namespace Interpolator.Encoding
{
   internal sealed class EncodingProgressEventArgs : EventArgs
   {
      public EncodingProgressEventArgs( int framesDone )
      {
         FramesDone = framesDone;
      }

      public int FramesDone { get; }
   }
}
