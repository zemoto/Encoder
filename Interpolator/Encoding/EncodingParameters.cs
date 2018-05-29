using System;

namespace Interpolator.Encoding
{
   internal struct EncodingParameters
   {
      public EncodingParameters( 
         string sourceFile,
         double sourceFrameRate,
         TimeSpan sourceDuration,
         string targetFile,
         double targetFrameRate )
      {
         SourceFile = sourceFile;
         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
         TargetFile = targetFile;
         TargetFrameRate = targetFrameRate;
      }

      public string SourceFile { get; }
      public double SourceFrameRate { get; }
      public TimeSpan SourceDuration { get; }

      public string TargetFile { get; }
      public double TargetFrameRate { get; }

      public bool ShouldInterpolate => Math.Abs( SourceFrameRate - TargetFrameRate ) > 5;
      public int TargetTotalFrames => (int)( SourceDuration.TotalSeconds * ( ShouldInterpolate ? TargetFrameRate : SourceFrameRate ) );
   }
}
