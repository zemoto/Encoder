using System.Drawing;
using System.Windows.Input;
using VideoInterpolator.Utils;

namespace VideoInterpolator
{
   internal sealed class MainViewModel : NotifyPropertyChanged
   {
      private Bitmap _frame;
      public Bitmap Frame
      {
         get => _frame;
         set => SetProperty( ref _frame, value );
      }

      public ICommand SelectVideoCommand { get; set; }
      public ICommand NextFrameCommand { get; set; }
   }
}
