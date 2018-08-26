using System;
using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Utils;

namespace Encoder.Encoding
{
   internal sealed class EncodingManagerViewModel : ViewModelBase
   {
      public void UpdateState( TimeSpan timeRemaining )
      {
         SetProperty( ref _timeRemaining, timeRemaining, nameof( TimeRemainingString ) );
         OnPropertyChanged( nameof( CpuUsage ) );
      }

      public ObservableCollection<EncodingTaskViewModel> Tasks { get; } = new ObservableCollection<EncodingTaskViewModel>();

      public bool AnyTasksPending => Tasks.Any();
      public bool NoTasksStarted => Tasks.Any( x => !x.Started );
      public bool AllTasksStarted => Tasks.All( x => x.Started );
      public EncodingTaskViewModel NextPendingTask => Tasks.FirstOrDefault( x => !x.Started );

      private TimeSpan _timeRemaining;
      public string TimeRemainingString
      {
         get
         {
            if ( _timeRemaining == TimeSpan.Zero || Tasks.Any( x => x.HasNoDurationData ) )
            {
               return "N/A";
            }
            return _timeRemaining.ToString( @"hh\:mm\:ss" );
         }
      }

      public int CpuUsage => Tasks.Any() ? Tasks.Select( x => x.CpuUsage ).Aggregate( ( x, y ) => x += y ) : 0;
   }
}
