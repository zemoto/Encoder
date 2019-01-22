using System;
using System.IO;
using System.Threading;
using System.Windows;
using ZemotoCommon.UI;
using ZemotoCommon.Utils;

namespace Encoder.Encoding.EncodingTask
{
   internal abstract class EncodingTaskBase : ViewModelBase, IDisposable
   {
      protected TimeSpan SourceDuration;
      protected double SourceFrameRate;
      public CancellationTokenSource CancelToken { get; } = new CancellationTokenSource();

      protected EncodingTaskBase( string sourceFile )
      {
         SourceFile = sourceFile;
      }

      public void Dispose()
      {
         CancelToken?.Dispose();
      }

      public virtual bool Initialize()
      {
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
         TargetFile = Path.ChangeExtension( UtilityMethods.MakeUniqueFileName( fullPath ), null );

         if ( !Directory.Exists( dir ) )
         {
            Directory.CreateDirectory( dir );
         }

         return true;
      }

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

      public abstract string EncodingArgs { get; }
      public abstract string TargetFileExtension { get; }
      public abstract string TaskName { get; }
      public string SourceFile { get; }
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
