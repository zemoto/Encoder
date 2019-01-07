using Encoder.Encoding;
using Encoder.Encoding.EncodingTask;
using Encoder.TaskCreation;
using ZemotoCommon.UI;

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

      public RelayCommand<EncodingTaskBase> CancelTaskCommand { get; set; }
   }
}
