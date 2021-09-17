using System;
using System.IO;
using ZemotoUI;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTaskBase : ViewModelBase, IFilePathProvider, IDisposable
   {
      public abstract bool DoWork();
      public abstract void Cleanup();
      public abstract void Cancel();
      public abstract void Dispose();

      public virtual string GetFilePath() => FileProvider.GetFilePath();

      private double _framesProcessedPerSecond = double.NaN;
      private void UpdateProgress()
      {
         if ( !Started || UnableToShowProgress || FramesDone == 0 )
         {
            return;
         }

         // Exponential Moving Average
         const double processingSpeedConstant = 0.005;
         var ellapsed = DateTime.Now - StartTime;
         var currentFramesProcessedPerSecond = FramesDone / ellapsed.TotalSeconds;

         _framesProcessedPerSecond = double.IsNaN( _framesProcessedPerSecond )
            ? currentFramesProcessedPerSecond
            : ( processingSpeedConstant * currentFramesProcessedPerSecond ) + ( ( 1 - processingSpeedConstant ) * _framesProcessedPerSecond );

         var framesLeft = TargetTotalFrames - FramesDone;
         _timeRemaining = TimeSpan.FromSeconds( framesLeft / _framesProcessedPerSecond );
         Progress = Math.Round( FramesDone / (double)TargetTotalFrames * 100, 2 );

         OnPropertyChanged( nameof( TimeRemainingString ) );
      }

      public abstract string TaskName { get; }

      public string Error { get; protected set; }

      private VideoMetadata _sourceMetadata;
      public VideoMetadata SourceMetadata
      {
         get => _sourceMetadata;
         protected set
         {
            if ( SetProperty( ref _sourceMetadata, value ) )
            {
               OnPropertyChanged( nameof( UnableToShowProgress ) );
            }
         }
      }

      private int _cpuUsage;
      public int CpuUsage
      {
         get => _cpuUsage;
         set => SetProperty( ref _cpuUsage, value );
      }

      protected DateTime StartTime;
      private bool _started;
      public bool Started
      {
         get => _started;
         set
         {
            if ( SetProperty( ref _started, value ) && value )
            {
               StartTime = DateTime.Now;
            }
         }
      }

      public string FileName => Path.GetFileName( SourceFile );

      private TimeSpan _timeRemaining = TimeSpan.Zero;
      public string TimeRemainingString => _timeRemaining == TimeSpan.Zero ? "N/A" : _timeRemaining.ToString( @"hh\:mm\:ss" );

      public IFilePathProvider FileProvider { get; set; }
      public string SourceFile => FileProvider.GetFilePath();

      public bool UnableToShowProgress => SourceMetadata == null || SourceMetadata.Duration == TimeSpan.Zero || TargetTotalFrames == 0;

      private int _targetTotalFrames;
      public int TargetTotalFrames
      {
         get => _targetTotalFrames;
         protected set
         {
            if ( SetProperty( ref _targetTotalFrames, value ) )
            {
               OnPropertyChanged( nameof( UnableToShowProgress ) );
            }
         }
      }

      private int _framesDone;
      public int FramesDone
      {
         get => _framesDone;
         set
         {
            if ( SetProperty( ref _framesDone, value ) )
            {
               UpdateProgress();
            }
         }
      }

      private double _progress;
      public double Progress
      {
         get => _progress;
         private set => SetProperty( ref _progress, value );
      }
   }
}
