using Encoder.Encoding;
using Encoder.TaskCreation;
using Encoder.Utils;

namespace Encoder
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public MainWindowViewModel( EncodingManagerViewModel encodingViewModel, TaskCreationViewModel taskCreationVm )
      {
         EncodingVm = encodingViewModel;
         TaskCreationVm = taskCreationVm;
      }

      public EncodingManagerViewModel EncodingVm { get; }
      public TaskCreationViewModel TaskCreationVm { get; }

      public RelayCommand<EncodingTaskViewModel> CancelTaskCommand { get; set; }
   }
}
