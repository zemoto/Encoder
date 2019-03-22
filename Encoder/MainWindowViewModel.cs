using Encoder.AssemblyLineCreation;
using Encoder.Encoding;
using Encoder.Encoding.Tasks;
using ZemotoCommon.UI;

namespace Encoder
{
   internal sealed class MainWindowViewModel : ViewModelBase
   {
      public MainWindowViewModel( EncodingManagerViewModel encodingViewModel, AssemblyLineCreationViewModel assemblyLineCreationVm )
      {
         EncodingVm = encodingViewModel;
         AssemblyLineCreationVm = assemblyLineCreationVm;
      }

      public EncodingManagerViewModel EncodingVm { get; }
      public AssemblyLineCreationViewModel AssemblyLineCreationVm { get; }

      public RelayCommand<EncodingTask> CancelTaskCommand { get; set; }
   }
}
