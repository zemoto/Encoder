using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJobViewModel : ViewModelBase
   {
      public EncodingJobViewModel( IEnumerable<EncodingTaskViewModel> tasks )
      {
         Tasks = new ObservableCollection<EncodingTaskViewModel>( tasks );
      }

      public void UpdateJobState( TimeSpan timeRemaining, int cpuUsage )
      {
         SetProperty( ref _timeRemaining, timeRemaining, nameof( TimeRemainingString ) );
         SetProperty( ref _cpuUsage, cpuUsage, nameof( CpuUsage ) );
      }

      public ObservableCollection<EncodingTaskViewModel> Tasks { get; }

      private EncodingTaskViewModel _currentTask;
      public EncodingTaskViewModel CurrentTask
      {
         get => _currentTask;
         set => SetProperty( ref _currentTask, value );
      }

      private TimeSpan _timeRemaining;
      public string TimeRemainingString
      {
         get
         {
            if ( CurrentTask?.HasNoDurationData == true )
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

      private int _cpuUsage;
      public int CpuUsage => _cpuUsage;

      public RelayCommand StopJobCommand { get; set; }
   }
}
