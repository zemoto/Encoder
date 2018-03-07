using System;
using System.Windows.Input;
using VideoInterpolator.Utils;

namespace VideoInterpolator
{
   internal sealed class MainViewModel : NotifyPropertyChanged
   {
      private int _framesDone;

      public int FramesDone
      {
         get => _framesDone;
         set
         {
            _framesDone = value;
            PercentDone = Math.Round( FramesDone / (double)TotalFrames * 100, 2 );
         }
      }

      public long TotalFrames { get; set; }

      private double _percentDone;
      public double PercentDone
      {
         get => _percentDone;
         set => SetProperty( ref _percentDone, value );
      }

      public ICommand SelectVideoCommand { get; set; }
      public ICommand DoInterpolationCommand { get; set; }
   }
}
