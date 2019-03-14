using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding.Tasks;
using ZemotoCommon.UI;

namespace Encoder.Encoding
{
   internal sealed class EncodingManagerViewModel : ViewModelBase
   {
      public ObservableCollection<SingleStepTask> Tasks { get; } = new ObservableCollection<SingleStepTask>();

      public bool AnyTasksPending => Tasks.Any();
      public bool NoTasksStarted => Tasks.Any( x => !x.Started );
      public bool AllTasksStarted => Tasks.All( x => x.Started );
      public SingleStepTask NextPendingTask => Tasks.FirstOrDefault( x => !x.Started );
   }
}
