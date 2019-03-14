using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Encoder.Encoding.Tasks;
using Encoder.Filters.Audio;
using Encoder.Filters.Audio.Copy;
using Encoder.Filters.Video;
using Encoder.Filters.Video.Copy;
using Encoder.Operations;
using ZemotoCommon.UI;

namespace Encoder.TaskCreation
{
   internal sealed class TaskCreationViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();

      public IEnumerable<EncodingTaskBase> GetTasks()
      {
         IEnumerable<EncodingTaskBase> tasks;
         if ( OperationType == OperationType.Filters )
         {
            tasks = SelectedFiles.Select( x => new EncodeWithFilters( x, VideoFilter, AudioFilter ) ).ToList();
         }
         else
         {
            tasks = SelectedFiles.Select( file => new MultiStepTask( 
               Operation.Steps.Select( step => new TaskStep( step.Step, new EncodeWithCustomParams( file, step.Params ) ) ).ToList() 
               ) ).ToList();
         }

         SelectedFiles.Clear();
         OperationType = OperationType.Filters;
         VideoFilterType = VideoFilterType.Copy;
         AudioFilterType = AudioFilterType.Copy;

         return tasks;
      }

      private OperationType _operationType;
      public OperationType OperationType
      {
         get => _operationType;
         set
         {
            if ( SetProperty( ref _operationType, value ) )
            {
               Operation = Operation.GetOperationForType( value );
            }
         }
      }

      private Operation _operation;
      public Operation Operation
      {
         get => _operation;
         set => SetProperty( ref _operation, value );
      }

      private VideoFilterType _videoFilterType = VideoFilterType.Copy;
      public VideoFilterType VideoFilterType
      {
         get => _videoFilterType;
         set
         {
            if ( SetProperty( ref _videoFilterType, value ) )
            {
               VideoFilter = VideoFilter.GetFilterForType( value );
            }
         }
      }

      private VideoFilter _videoFilter = new CopyVideoFilter();
      public VideoFilter VideoFilter
      {
         get => _videoFilter;
         private set => SetProperty( ref _videoFilter, value );
      }

      private AudioFilterType _audioFilterType = AudioFilterType.Copy;
      public AudioFilterType AudioFilterType
      {
         get => _audioFilterType;
         set
         {
            if ( SetProperty( ref _audioFilterType, value ) )
            {
               AudioFilter = AudioFilter.GetFilterForType( value );
            }
         }
      }

      private AudioFilter _audioFilter = new CopyAudioFilter();
      public AudioFilter AudioFilter
      {
         get => _audioFilter;
         private set => SetProperty( ref _audioFilter, value );
      }

      public RelayCommand SelectFilesCommand { get; set; }
      public RelayCommand<string> RemoveFileCommand { get; set; }
      public RelayCommand CreateTasksCommand { get; set; }
   }
}
