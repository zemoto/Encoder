using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Utils;

namespace Encoder.Encoding
{
   internal sealed class EncodingManagerViewModel : ViewModelBase
   {
      public ObservableCollection<EncodingTaskViewModel> Tasks { get; } = new ObservableCollection<EncodingTaskViewModel>();

      public bool AnyTasksPending => Tasks.Any();
      public bool NoTasksStarted => Tasks.Any( x => !x.Started );
      public bool AllTasksStarted => Tasks.All( x => x.Started );
      public EncodingTaskViewModel NextPendingTask => Tasks.FirstOrDefault( x => !x.Started );
   }
}
