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
      private readonly string _assemblyLineRootDirectory;
      private readonly string _assemblyLineWorkingDirectory;
      private readonly string _assemblyLineId;

      public AssemblyLine( IFilePathProvider sourceFilePathProvider, EncodingTask[] steps, int id )
      {
         SourceFilePathProvider = sourceFilePathProvider;
         _steps = steps;
         _assemblyLineId = $"{Path.GetFileNameWithoutExtension( SourceFile )}-{id}";
         _assemblyLineRootDirectory = Path.Combine( Path.GetDirectoryName( SourceFile ), "done" );
         _assemblyLineWorkingDirectory = Path.Combine( _assemblyLineRootDirectory, _assemblyLineId );
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
         UtilityMethods.CreateDirectory( _assemblyLineWorkingDirectory );
         foreach ( var task in _steps )
         {
            if ( !DoTask( task ) )
            {
               return false;
            }

            SourceFilePathProvider = task;
         }

         var finalFile = GetFilePath();
         finalFile = PathUtils.MoveFileToFolder( finalFile, _assemblyLineRootDirectory );
         finalFile = PathUtils.RenameFile( finalFile, _assemblyLineId );
         UtilityMethods.SafeDeleteDirectory( _assemblyLineWorkingDirectory );

         SourceFilePathProvider = new FilePathProvider( finalFile );

         return true;
      }

      private bool DoTask( EncodingTask task )
      {
         task.SourceFilePathProvider = SourceFilePathProvider;

         if ( !task.Initialize( _assemblyLineWorkingDirectory, _stepsFinished++ ) )
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

         UtilityMethods.SafeDeleteDirectory( _assemblyLineWorkingDirectory );
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
