namespace Encoder
{
   internal partial class MainWindow
   {
      public MainWindow( MainWindowViewModel model )
      {
         DataContext = model;
         InitializeComponent();
      }

      private void OnSizeChanged( object sender, System.Windows.SizeChangedEventArgs e )
      {
         if ( e.HeightChanged )
         {
            var heightDifference = e.NewSize.Height - e.PreviousSize.Height;
            Top -= heightDifference / 2.0;
         }
      }
   }
}
