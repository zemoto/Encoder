using Encoder.TaskCreation;
using Encoder.Encoding;
using Encoder.Encoding.Tasks;
using ZemotoUI;

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
