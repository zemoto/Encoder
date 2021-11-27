using System.Windows;
using ZemotoUtils;

namespace Encoder
{
   internal partial class App
   {
      private Main _main;

      protected override void OnStartup( StartupEventArgs e )
      {
         ChildProcessWatcher.Initialize();

         _main = new Main();
         _main.Show();
      }

      protected override void OnExit( ExitEventArgs e ) => _main.Dispose();
   }
}
