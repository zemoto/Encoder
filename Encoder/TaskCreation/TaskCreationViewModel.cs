using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding;
using Encoder.Filters;
using Encoder.Utils;

namespace Encoder.TaskCreation
{
   internal sealed class TaskCreationViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();

      public IEnumerable<EncodingTaskViewModel> GetTasks()
      {
         var files = SelectedFiles.ToList();
         var tasks = files.Select( x => new EncodingTaskViewModel( x, Reencode, Filter ) );
         SelectedFiles.Clear();
         SelectedFilter = FilterType.None;

         return tasks;
      }

      private bool _reencode = true;
      public bool Reencode
      {
         get => _reencode;
         set => SetProperty( ref _reencode, value );
      }

      private FilterType _selectedFilter = FilterType.None;
      public FilterType SelectedFilter
      {
         get => _selectedFilter;
         set
         {
            if ( SetProperty( ref _selectedFilter, value ) )
            {
               Filter = FilterProvider.GetFilterForType( value );
            }
         }
      }

      private Filter _filter;
      public Filter Filter
      {
         get => _filter;
         private set => SetProperty( ref _filter, value );
      }

      public RelayCommand SelectFilesCommand { get; set; }
      public RelayCommand<string> RemoveFileCommand { get; set; }
      public RelayCommand CreateTasksCommand { get; set; }
   }
}
