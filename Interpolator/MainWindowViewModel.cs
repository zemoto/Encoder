using Interpolator.Encoding;
using Interpolator.Utils;

namespace Interpolator
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public MainWindowViewModel( EncodingManagerViewModel encodingViewModel )
      {
         EncodingVm = encodingViewModel;
      }

      public EncodingManagerViewModel EncodingVm { get; }

      public RelayCommand NewTasksCommand { get; set; }
   }
}
