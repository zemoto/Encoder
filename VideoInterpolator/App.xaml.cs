using System.Windows;

namespace VideoInterpolator
{
   public partial class App
   {
      protected override void OnStartup( StartupEventArgs e )
      {
         base.OnStartup( e );

         var main = new Main();
         main.ShowDialog();
      }
   }
}
