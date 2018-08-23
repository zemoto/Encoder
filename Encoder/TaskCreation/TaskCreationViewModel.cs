using System.Collections.ObjectModel;
using Encoder.Filters;
using Encoder.Utils;

namespace Encoder.TaskCreation
{
   internal sealed class TaskCreationViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();

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
