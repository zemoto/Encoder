using Interpolator.Utils;

namespace Interpolator
{
   internal partial class App
   {
      public App()
      {
         ChildProcessWatcher.Initialize();
         TotalCpuMonitor.Initialize();

         var main = new Main();
         main.ShowDialog();
      }
   }
}
