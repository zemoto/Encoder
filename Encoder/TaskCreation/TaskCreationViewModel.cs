using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding;
using Encoder.Filters.Video;
using Encoder.Utils;

namespace Encoder.TaskCreation
{
   internal sealed class TaskCreationViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();

      public IEnumerable<EncodingTaskViewModel> GetTasks()
      {
         var tasks = SelectedFiles.Select( x => new EncodingTaskViewModel( x, ReEncode, VideoFilter ) ).ToList();
         SelectedFiles.Clear();
         SelectedFilter = VideoFilterType.None;
         return tasks;
      }

      private bool _reEncode = true;
      public bool ReEncode
      {
         get => _reEncode;
         set => SetProperty( ref _reEncode, value );
      }

      private VideoFilterType _selectedFilter = VideoFilterType.None;
      public VideoFilterType SelectedFilter
      {
         get => _selectedFilter;
         set
         {
            if ( SetProperty( ref _selectedFilter, value ) )
            {
               VideoFilter = VideoFilterProvider.GetFilterForType( value );
            }
         }
      }

      private VideoFilter _videoFilter;
      public VideoFilter VideoFilter
      {
         get => _videoFilter;
         private set => SetProperty( ref _videoFilter, value );
      }

      public RelayCommand SelectFilesCommand { get; set; }
      public RelayCommand<string> RemoveFileCommand { get; set; }
      public RelayCommand CreateTasksCommand { get; set; }
   }
}
