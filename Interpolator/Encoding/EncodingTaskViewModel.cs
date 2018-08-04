using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Interpolator.Filters;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal sealed class EncodingTaskViewModel : ViewModelBase
   {
      private Filter _filter;
      private double _sourceFrameRate;
      private TimeSpan _sourceDuration;

      public EncodingTaskViewModel( string sourceFile, Filter filter )
      {
         SourceFile = sourceFile;
         _filter = filter;
      }

      private bool ShouldApplyFilter() => _filter != null && _filter.ShouldApplyFilter();

      public string GetEncodingArguments() => ShouldApplyFilter() ? _filter.GetFilterArguments() : null;

      public async Task<bool> InitializeTaskAsync()
      {
         bool success = false;
         double sourceFrameRate = 0;
         var sourceDuration = TimeSpan.Zero;
         await Task.Run( () => success = VideoMetadataReader.GetVideoInfo( SourceFile, out sourceFrameRate, out sourceDuration ) );
         if ( !success )
         {
            MessageBox.Show( $"Could not read video file: {SourceFile}" );
            return false;
         }

         _sourceFrameRate = sourceFrameRate;
         _sourceDuration = sourceDuration;
         TargetFile = Path.Combine( Path.GetDirectoryName( SourceFile ), Path.GetFileNameWithoutExtension( SourceFile ) + $"_done.mp4" );

         _filter?.Initialize( sourceFrameRate, sourceDuration );

         OnPropertyChanged( null );

         return true;
      }

      public string FilterName => ShouldApplyFilter() ? _filter.GetFilterName() : "None";
      public string SourceFile { get; }
      public string FileName => Path.GetFileName( SourceFile );
      public bool HasNoDurationData => _sourceDuration == TimeSpan.Zero && !Finished;

      public string TargetFile { get; private set; }

      public int TargetTotalFrames => ShouldApplyFilter() ? _filter.GetTargetFrameCount() : (int)( _sourceDuration.TotalSeconds * _sourceFrameRate );

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
