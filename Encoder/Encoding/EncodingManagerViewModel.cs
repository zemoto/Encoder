using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding.Tasks;
using ZemotoCommon.UI;

namespace Encoder.Encoding
{
   internal sealed class EncodingManagerViewModel : ViewModelBase
   {
      public ObservableCollection<EncodingTask> Tasks { get; } = new ObservableCollection<EncodingTask>();

      public bool AnyTasksPending => Tasks.Any();
      public bool NoTasksStarted => Tasks.Any( x => !x.Started );
      public bool AllTasksStarted => Tasks.All( x => x.Started );
      public EncodingTask NextPendingTask => Tasks.FirstOrDefault( x => !x.Started );
   }
}
