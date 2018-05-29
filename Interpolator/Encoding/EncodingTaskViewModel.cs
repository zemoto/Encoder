using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Interpolator.Encoding
{
   internal sealed class EncodingTaskViewModel : ViewModelBase
   {
      public EncodingTaskViewModel( IEnumerable<string> files, int targetFrameRate )
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

      public int _targetFrameRate;
      public int TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }

      public string _currentFileEllapsedEncodingTime = "00:00:00";
      public string CurrentFileEllapsedEncodingTime
      {
         get => _currentFileEllapsedEncodingTime;
         set => SetProperty( ref _currentFileEllapsedEncodingTime, value );
      }

      private string _encoderOutput;
      public string EncoderOutput
      {
         get => _encoderOutput;
         set => SetProperty( ref _encoderOutput, value );
      }

      public RelayCommand StopInterpolatingCommand { get; set; }
   }
}
