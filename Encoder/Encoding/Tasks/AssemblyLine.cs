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

      public AssemblyLine( IFilePathProvider fileProvider, EncodingTask[] steps )
      {
         FileProvider = fileProvider;
         _steps = steps;
         _assemblyLineId = $"{Path.GetFileNameWithoutExtension( SourceFile )}";
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

            FileProvider = task;
         }

         var finalFile = GetFilePath();
         FileUtils.MoveFileToFolder( finalFile, _assemblyLineRootDirectory, out finalFile );
         FileUtils.RenameFile( finalFile, _assemblyLineId, out finalFile );
         UtilityMethods.SafeDeleteDirectory( _assemblyLineWorkingDirectory );

         FileProvider = new FilePathProvider( finalFile );

         return true;
      }

      private bool DoTask( EncodingTask task )
      {
         task.FileProvider = FileProvider;

         if ( !task.Initialize( _assemblyLineWorkingDirectory, _stepsFinished++ ) )
         {
            Error = task.Error;
            return false;
         }

         CurrentTask = task;

         SourceDuration = CurrentTask.SourceDuration;
         SourceFrameRate = CurrentTask.SourceFrameRate;
         TargetTotalFrames = CurrentTask.TargetTotalFrames;

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

            if ( SetProperty( ref _currentTask, value ) )
            {
               FramesDone = _currentTask.FramesDone;
               CpuUsage = _currentTask.CpuUsage;
               OnPropertyChanged( nameof( TaskName ) );
            }

            if ( CurrentTask != null )
            {
               CurrentTask.PropertyChanged += OnCurrentTaskPropertyChanged;
            }
         }
      }
   }
}
