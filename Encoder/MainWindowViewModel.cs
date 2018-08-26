using Encoder.Encoding;
using Encoder.Utils;

namespace Encoder
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public MainWindowViewModel( EncodingManagerViewModel encodingViewModel )
      {
         EncodingVm = encodingViewModel;
      }

      public EncodingManagerViewModel EncodingVm { get; }

      public RelayCommand NewTasksCommand { get; set; }
      public RelayCommand<EncodingTaskViewModel> CancelTaskCommand { get; set; }
   }
}
