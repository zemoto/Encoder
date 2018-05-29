using System;

namespace Interpolator.Encoding
{
   internal sealed class EncodingProgressEventArgs : EventArgs
   {
      public EncodingProgressEventArgs( double progress )
      {
         Progress = progress;
      }

      public double Progress { get; }
   }
}
