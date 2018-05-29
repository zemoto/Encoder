using System.Collections.ObjectModel;
using Interpolator.Encoding;

namespace Interpolator
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();
      public ObservableCollection<EncodingTaskViewModel> EncodingTasks { get; } = new ObservableCollection<EncodingTaskViewModel>();

      public int _targetFrameRate = 60;
      public int TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }

      public RelayCommand SelectFilesCommand { get; set; }
      public RelayCommand StartTaskCommand { get; set; }
   }
}
