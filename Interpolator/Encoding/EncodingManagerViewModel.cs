using System;
using System.Collections.ObjectModel;
using System.Linq;
using Interpolator.Utils;

namespace Interpolator.Encoding
{
   internal sealed class EncodingManagerViewModel : ViewModelBase
   {
      public void UpdateState( TimeSpan timeRemaining )
      {
         SetProperty( ref _timeRemaining, timeRemaining, nameof( TimeRemainingString ) );
         OnPropertyChanged( nameof( CpuUsage ) );
      }

      public ObservableCollection<EncodingTaskViewModel> Tasks { get; } = new ObservableCollection<EncodingTaskViewModel>();

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
