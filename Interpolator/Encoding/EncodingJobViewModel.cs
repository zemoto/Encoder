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

      public ObservableCollection<EncodingTaskViewModel> Tasks { get; }

      private EncodingTaskViewModel _currentTask;
      public EncodingTaskViewModel CurrentTask
      {
         get => _currentTask;
         set => SetProperty( ref _currentTask, value );
      }

      private TimeSpan _timeRemaining;
      public void SetTimeRemaining( TimeSpan remaining )
      {
         if ( remaining == _timeRemaining )
         {
            return;
         }
         _timeRemaining = remaining;
         OnPropertyChanged( nameof( TimeRemaining ) );
      }

      public string TimeRemaining
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

      public RelayCommand StopJobCommand { get; set; }
   }
}
