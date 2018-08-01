using System;

namespace Interpolator.Encoding
{
   internal sealed class EncodingProgressEventArgs : EventArgs
   {
      public EncodingProgressEventArgs( int framesDone, int currentCpuUsage )
      {
         FramesDone = framesDone;
         CurrentCpuUsage = currentCpuUsage;
      }

      public int FramesDone { get; }
      public int CurrentCpuUsage { get; }
   }
}
