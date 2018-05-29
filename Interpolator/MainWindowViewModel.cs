using System.Collections.ObjectModel;
using Interpolator.Encoding;

namespace Interpolator
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();
      public ObservableCollection<EncodingJobViewModel> EncodingJobs { get; } = new ObservableCollection<EncodingJobViewModel>();

      public int _targetFrameRate = 60;
      public int TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }

      public RelayCommand SelectFilesCommand { get; set; }
      public RelayCommand<string> RemoveFileCommand { get; set; }
      public RelayCommand StartJobCommand { get; set; }
   }
}
