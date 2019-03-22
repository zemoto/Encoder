using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using ZemotoCommon.Utils;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTask : EncodingTaskBase, IDisposable
   {
      protected TimeSpan SourceDuration;
      protected double SourceFrameRate;

      ~EncodingTask()
      {
         Dispose();
      }

      public void Dispose() => CancelToken?.Dispose();

      public override void Cleanup() => UtilityMethods.SafeDeleteFile( TargetFile );

      public override void Cancel() => CancelToken.Cancel();

      public override string GetFilePath() => TargetFile;

      public virtual bool Initialize()
      {
         Debug.Assert( SourceFilePathProvider != null );
         bool success = VideoMetadataReader.GetVideoInfo( SourceFile, out var sourceFrameRate, out var sourceDuration );
         if ( !success )
         {
            MessageBox.Show( $"Could not read video file: {SourceFile}" );
            return false;
         }

         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
         TargetTotalFrames = (int)Math.Ceiling( SourceFrameRate * SourceDuration.TotalSeconds );

         var dir = Path.Combine( Path.GetDirectoryName( SourceFile ), "done" );
         var fullPath = Path.Combine( dir, Path.GetFileName( SourceFile ) );
         TargetFile = UtilityMethods.MakeUniqueFileName( Path.ChangeExtension( fullPath, TargetFileExtension ) );

         if ( !Directory.Exists( dir ) )
         {
            Directory.CreateDirectory( dir );
         }

         return true;
      }

      public void SetTaskFinished() => RaiseTaskFinished( Started && !CancelToken.IsCancellationRequested );

      private void UpdateTimeRemaining()
      {
         if ( !Started || HasNoDurationData || FramesDone == 0 )
         {
            return;
         }

         var ellapsed = DateTime.Now - _startTime;
         _timeRemaining = TimeSpan.FromSeconds( ellapsed.TotalSeconds / FramesDone * ( TargetTotalFrames - FramesDone ) );

         OnPropertyChanged( nameof( TimeRemainingString ) );
      }

      public CancellationTokenSource CancelToken { get; } = new CancellationTokenSource();
      public abstract string EncodingArgs { get; }
      public abstract string TargetFileExtension { get; }
      public abstract string TaskName { get; }
      public string SourceFile => SourceFilePathProvider.GetFilePath();
      public string FileName => Path.GetFileName( SourceFile );
      public bool HasNoDurationData => SourceDuration == TimeSpan.Zero;
      public string TargetFile { get; private set; }
      public int TargetTotalFrames { get; protected set; }

      private DateTime _startTime;
      private TimeSpan _timeRemaining = TimeSpan.Zero;
      public string TimeRemainingString => _timeRemaining == TimeSpan.Zero ? "N/A" : _timeRemaining.ToString( @"hh\:mm\:ss" );

      private int _cpuUsage;
      public int CpuUsage
      {
         get => _cpuUsage;
         set => SetProperty( ref _cpuUsage, value );
      }

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
                  UpdateTimeRemaining();
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

      private bool _started;
      public bool Started
      {
         get => _started;
         set
         {
            if ( SetProperty( ref _started, value ) && value )
            {
               _startTime = DateTime.Now;
            }
         }
      }
   }
}
