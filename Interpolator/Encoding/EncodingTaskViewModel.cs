using System;
using System.IO;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal sealed class EncodingTaskViewModel : ViewModelBase
   {
      public EncodingTaskViewModel( string sourceFile, double targetFrameRate )
      {
         if ( !VideoMetadataReader.GetVideoInfo( sourceFile, out double sourceFrameRate, out TimeSpan duration ) )
         {
            throw new ArgumentException( "Could not read file", nameof( sourceFile ) );
         }

         var targetDir = Path.Combine( Path.GetDirectoryName( sourceFile ), "interpolated" );

         SourceFile = sourceFile;
         SourceFrameRate = sourceFrameRate;
         SourceDuration = duration;
         TargetFile = Path.Combine( targetDir, Path.GetFileNameWithoutExtension( sourceFile ) + ".mp4" );

         // Target the closest framerate that is multiple of half the original framerate.
         // This should prevent interpolator from having to put in weird partial frames.
         var halfSourceFrameRate = sourceFrameRate / 2;
         TargetFrameRate = UtilityMethods.GetClosestMultiple( halfSourceFrameRate, targetFrameRate );
      }

      public string SourceFile { get; }
      public string FileName => Path.GetFileName( SourceFile );
      public double SourceFrameRate { get; }
      public TimeSpan SourceDuration { get; }
      public bool HasNoDurationData => SourceDuration == TimeSpan.Zero;

      public string TargetFile { get; }
      public double TargetFrameRate { get; }

      public bool ShouldInterpolate => TargetFrameRate / SourceFrameRate > 1.5;
      public int TargetTotalFrames => (int)( SourceDuration.TotalSeconds * ( ShouldInterpolate ? TargetFrameRate : SourceFrameRate ) );

      private int _framesDone;
      public int FramesDone
      {
         get => _framesDone;
         set
         {
            if ( SetProperty( ref _framesDone, value ) )
            {
               if ( value >= TargetTotalFrames )
               {
                  Finished = true;
                  Progress = 100;
               }
               else
               {
                  Progress = Math.Round( value / (double)TargetTotalFrames * 100, 2 );
               }
            }
         }
      }

      private double _progress;
      public double Progress
      {
         get => _progress;
         private set => SetProperty( ref _progress, value );
      }

      private bool _finished;
      public bool Finished
      {
         get => _finished;
         private set => SetProperty( ref _finished, value );
      }

      private bool _started;
      public bool Started
      {
         get => _started;
         set => SetProperty( ref _started, value );
      }
   }
}
