using System.Windows.Input;
using VideoInterpolator.Utils;

namespace VideoInterpolator
{
   internal sealed class MainViewModel : NotifyPropertyChanged
   {
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
