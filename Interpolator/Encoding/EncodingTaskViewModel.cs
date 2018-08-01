using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal sealed class EncodingTaskViewModel : ViewModelBase
   {
      public EncodingTaskViewModel( string sourceFile, double targetFrameRate )
      {
         SourceFile = sourceFile;
         TargetFrameRate = targetFrameRate;
      }

      public async Task<bool> InitializeTaskAsync()
      {
         bool success = false;
         double sourceFrameRate = 0;
         var duration = TimeSpan.Zero;
         await Task.Run( () => success = VideoMetadataReader.GetVideoInfo( SourceFile, out sourceFrameRate, out duration ) );
         if ( !success )
         {
            MessageBox.Show( $"Could not read video file: {SourceFile}" );
            return false;
         }

         SourceFrameRate = sourceFrameRate;
         SourceDuration = duration;
         TargetFile = Path.Combine( Path.GetDirectoryName( SourceFile ), Path.GetFileNameWithoutExtension( SourceFile ) + $"_{TargetFrameRate}.mp4" );

         // Target the closest framerate that is multiple of half the original framerate.
         // This should prevent interpolator from having to put in weird partial frames.
         TargetFrameRate = UtilityMethods.GetClosestMultiple( sourceFrameRate / 2, TargetFrameRate );

         OnPropertyChanged( null );

         return true;
      }

      public string SourceFile { get; }
      public string FileName => Path.GetFileName( SourceFile );
      public double SourceFrameRate { get; private set; }
      public TimeSpan SourceDuration { get; private set; }
      public bool HasNoDurationData => SourceDuration == TimeSpan.Zero && !Finished;

      public string TargetFile { get; private set; }
      public double TargetFrameRate { get; private set; }

      public bool ShouldInterpolate => TargetFrameRate / SourceFrameRate > 1.5;
      public int TargetTotalFrames => (int)( SourceDuration.TotalSeconds * ( ShouldInterpolate ? TargetFrameRate : SourceFrameRate ) );

      public int CpuUsage { get; set; }

      private int _framesDone;
      public int FramesDone
      {
         get => _framesDone;
         set
         {
            if ( SetProperty( ref _framesDone, value ) )
            {
               if ( TargetTotalFrames != 0 )
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
         set
         {
            if ( SetProperty( ref _finished, value ) )
            {
               OnPropertyChanged( nameof( HasNoDurationData ) );
               if ( value )
               {
                  FramesDone = TargetTotalFrames;
                  Progress = 100;
               }
            }
         }
      }

      private bool _started;
      public bool Started
      {
         get => _started;
         set => SetProperty( ref _started, value );
      }
   }
}
