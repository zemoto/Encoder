using System.Collections.ObjectModel;
using Interpolator.Filters;
using Interpolator.Utils;

namespace Interpolator.JobCreation
{
   internal sealed class JobCreationViewModel : ViewModelBase
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
      public RelayCommand CreateJobCommand { get; set; }
   }
}
