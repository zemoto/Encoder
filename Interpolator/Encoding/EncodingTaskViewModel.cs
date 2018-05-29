using System;
using System.IO;

namespace Interpolator.Encoding
{
   internal sealed class EncodingTaskViewModel : ViewModelBase
   {
      public EncodingTaskViewModel( string sourceFile, double targetFrameRate )
      {
         if ( !FfmpegEncoder.GetVideoInfo( sourceFile, out double frameRate, out TimeSpan duration ) )
         {
            throw new ArgumentException( "Could not read file", nameof( sourceFile ) );
         }

         var targetDir = Path.Combine( Path.GetDirectoryName( sourceFile ), "interpolated" );

         SourceFile = sourceFile;
         SourceFrameRate = frameRate;
         SourceDuration = duration;
         TargetFile = Path.Combine( targetDir, Path.GetFileNameWithoutExtension( sourceFile ) + ".mp4" );
         TargetFrameRate = targetFrameRate;
      }

      public string SourceFile { get; }
      public double SourceFrameRate { get; }
      public TimeSpan SourceDuration { get; }

      public string TargetFile { get; }
      public double TargetFrameRate { get; }

      public bool ShouldInterpolate => Math.Abs( SourceFrameRate - TargetFrameRate ) > 5;
      public int TargetTotalFrames => (int)( SourceDuration.TotalSeconds * ( ShouldInterpolate ? TargetFrameRate : SourceFrameRate ) );

      private double _progress;
      public double Progress
      {
         get => _progress;
         set => SetProperty( ref _progress, value );
      }
   }
}
