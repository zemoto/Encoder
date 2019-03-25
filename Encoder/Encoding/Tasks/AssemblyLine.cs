using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using ZemotoCommon.Utils;

namespace Encoder.Encoding.Tasks
{
   internal sealed class AssemblyLine : EncodingTaskBase
   {
      private int _stepsFinished;
      private readonly EncodingTask[] _steps;
      private readonly string _assemblyLineDirectory;

      private readonly string _assemblyLineId = Guid.NewGuid().ToString( "N" ).Substring( 0, 8 );

      public AssemblyLine( IFilePathProvider sourceFilePathProvider, EncodingTask[] steps )
      {
         SourceFilePathProvider = sourceFilePathProvider;
         _steps = steps;
         _assemblyLineDirectory = Path.Combine( Path.GetDirectoryName( SourceFile ), "done", _assemblyLineId );
         CurrentTask = _steps.First(); // So correct information displays before this task gets started
      }

      private void OnCurrentTaskPropertyChanged( object sender, PropertyChangedEventArgs e )
      {
         switch ( e.PropertyName )
         {
            case nameof( CpuUsage ):
               CpuUsage = CurrentTask.CpuUsage;
               break;
            case nameof( FramesDone ):
               FramesDone = CurrentTask.FramesDone;
               break;
         }
      }

      public override bool DoWork()
      {
         Started = true;
         EncodingTask previousTask = null;
         UtilityMethods.CreateDirectory( _assemblyLineDirectory );
         foreach ( var task in _steps )
         {
            if ( !DoTask( task, previousTask ) )
            {
               return false;
            }
            previousTask = task;
         }

         return true;
      }

      private bool DoTask( EncodingTask task, IFilePathProvider sourceFilePathProvider )
      {
         task.SourceFilePathProvider = sourceFilePathProvider ?? this;

         if ( !task.Initialize( _assemblyLineDirectory, _stepsFinished++ ) )
         {
            Error = task.Error;
            return false;
         }

         CurrentTask = task;

         SourceDuration = CurrentTask.SourceDuration;
         SourceFrameRate = CurrentTask.SourceFrameRate;
         TargetTotalFrames = CurrentTask.TargetTotalFrames;
         FramesDone = 0;
         CpuUsage = 0;
         OnPropertyChanged( nameof( TaskName ) );
         OnPropertyChanged( nameof( HasNoDurationData ) );

         bool success = CurrentTask.DoWork();
         if ( !success )
         {
            Error = CurrentTask.Error;
            return false;
         }

         return true;
      }

      public override void Cleanup()
      {
         Cancel();

         foreach( var step in _steps )
         {
            step.Cleanup();
         }

         UtilityMethods.SafeDeleteDirectory( _assemblyLineDirectory );
      }

      public override void Cancel() => CurrentTask?.Cancel();

      public override string GetFilePath() => SourceFilePathProvider.GetFilePath();

      public override void Dispose()
      {
         _steps.ForEach( x => x.Dispose() );
      }

      public override string TaskName => CurrentTask.TaskName;

      private EncodingTask _currentTask;
      public EncodingTask CurrentTask
      {
         get => _currentTask;
         private set
         {
            if ( CurrentTask != null )
            {
               CurrentTask.PropertyChanged -= OnCurrentTaskPropertyChanged;
            }

            SetProperty( ref _currentTask, value );

            if ( CurrentTask != null )
            {
               CurrentTask.PropertyChanged += OnCurrentTaskPropertyChanged;
            }
         }
      }
   }
}
