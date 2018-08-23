namespace Encoder
{
   internal partial class MainWindow
   {
      public MainWindow( MainWindowViewModel model )
      {
         DataContext = model;
         InitializeComponent();
      }
   }
}
