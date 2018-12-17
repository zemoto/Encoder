using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding;
using Encoder.Filters.Video;
using Encoder.Filters.Video.Copy;
using Encoder.Utils;

namespace Encoder.TaskCreation
{
   internal sealed class TaskCreationViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();

      public IEnumerable<EncodingTaskViewModel> GetTasks()
      {
         var tasks = SelectedFiles.Select( x => new EncodingTaskViewModel( x, VideoFilter ) ).ToList();
         SelectedFiles.Clear();
         SelectedFilter = VideoFilterType.Copy;
         return tasks;
      }

      private VideoFilterType _selectedFilter = VideoFilterType.Copy;
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

      private VideoFilter _videoFilter = new CopyVideoFilter();
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
