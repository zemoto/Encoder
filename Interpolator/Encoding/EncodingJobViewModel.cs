using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Interpolator.Encoding
{
   internal sealed class EncodingJobViewModel : ViewModelBase
   {
      public EncodingJobViewModel( IEnumerable<string> files, int targetFrameRate )
      {
         Files = new ObservableCollection<string>( files );
         TargetFrameRate = targetFrameRate;
      }

      public ObservableCollection<string> Files { get; }

      private string _currentFile;
      public string CurrentFile
      {
         get => _currentFile;
         set => SetProperty( ref _currentFile, value );
      }

      private int _targetFrameRate;
      public int TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
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
            if ( _timeRemaining == TimeSpan.Zero )
            {
               return "Estimating...";
            }
            return _timeRemaining.ToString( @"hh\:mm\:ss" );
         }
      }

      private bool _isInterpolating;
      public bool IsInterpolating
      {
         get => _isInterpolating;
         set => SetProperty( ref _isInterpolating, value );
      }

      private double _progress;
      public double Progress
      {
         get => _progress;
         set => SetProperty( ref _progress, value );
      }

      public RelayCommand StopInterpolatingCommand { get; set; }
   }
}
