using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJobViewModel : ViewModelBase
   {
      public EncodingJobViewModel( IEnumerable<EncodingTaskViewModel> tasks )
      {
         Tasks = new ObservableCollection<EncodingTaskViewModel>( tasks );
      }

      public void UpdateJobState( TimeSpan timeRemaining )
      {
         SetProperty( ref _timeRemaining, timeRemaining, nameof( TimeRemainingString ) );
         OnPropertyChanged( nameof( CpuUsage ) );
      }

      public ObservableCollection<EncodingTaskViewModel> Tasks { get; }

      public string JobName => Tasks.First().FileName;

      private TimeSpan _timeRemaining;
      public string TimeRemainingString
      {
         get
         {
            if ( Tasks.Any( x => x.HasNoDurationData ) )
            {
               return "N/A";
            }
            if ( _timeRemaining == TimeSpan.Zero )
            {
               return "Estimating...";
            }
            return _timeRemaining.ToString( @"hh\:mm\:ss" );
         }
      }

      public int CpuUsage => Tasks.Select( x => x.CpuUsage ).Aggregate( ( x, y ) => x += y );

      public RelayCommand StopJobCommand { get; set; }
   }
}
