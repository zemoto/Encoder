using System.Collections.ObjectModel;
using Interpolator.Encoding;
using Interpolator.Utils;

namespace Interpolator
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public ObservableCollection<EncodingJobViewModel> EncodingJobs { get; } = new ObservableCollection<EncodingJobViewModel>();

      public RelayCommand NewJobCommand { get; set; }
   }
}
