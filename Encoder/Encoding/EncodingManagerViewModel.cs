using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding.Tasks;
using ZemotoCommon.UI;

namespace Encoder.Encoding
{
   internal sealed class EncodingManagerViewModel : ViewModelBase
   {
      public ObservableCollection<EncodingTaskBase> Tasks { get; } = new ObservableCollection<EncodingTaskBase>();

      public bool AnyTasksPending => Tasks.Any();
      public bool NoTasksStarted => Tasks.Any( x => !x.Started );
      public bool AllTasksStarted => Tasks.All( x => x.Started );
      public EncodingTaskBase NextPendingTask => Tasks.FirstOrDefault( x => !x.Started );
   }
}
