using System;
using System.IO;
using ZemotoCommon.UI;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTaskBase : ViewModelBase, IFilePathProvider, IDisposable
   {
      public abstract bool DoWork();
      public abstract void Cleanup();
      public abstract void Cancel();
      public abstract void Dispose();

      public virtual string GetFilePath() => FileProvider.GetFilePath();

      private void UpdateProgress()
      {
         if ( !Started || HasNoDurationData || FramesDone == 0 || TargetTotalFrames == 0 )
         {
            return;
         }

         var ellapsed = DateTime.Now - StartTime;
         _timeRemaining = TimeSpan.FromSeconds( ellapsed.TotalSeconds / FramesDone * ( TargetTotalFrames - FramesDone ) );
         Progress = Math.Round( FramesDone / (double)TargetTotalFrames * 100, 2 );

         OnPropertyChanged( nameof( TimeRemainingString ) );
      }

      public abstract string TaskName { get; }

      public string Error { get; protected set; }

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

      public bool HasNoDurationData => SourceDuration == TimeSpan.Zero;

      private TimeSpan _sourceDuration;
      public TimeSpan SourceDuration
      {
         get => _sourceDuration;
         protected set
         {
            if ( SetProperty( ref _sourceDuration, value ) )
            {
               OnPropertyChanged( nameof( HasNoDurationData ) );
            }
         }
      }

      private double _sourceFrameRate;
      public double SourceFrameRate
      {
         get => _sourceFrameRate;
         protected set => SetProperty( ref _sourceFrameRate, value );
      }

      private int _targetTotalFrames;
      public int TargetTotalFrames
      {
         get => _targetTotalFrames;
         protected set => SetProperty( ref _targetTotalFrames, value );
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
