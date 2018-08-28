using Encoder.Utils;

namespace Encoder
{
   internal partial class App
   {
      public App()
      {
         ChildProcessWatcher.Initialize();
         TotalCpuMonitor.Initialize();

         using ( var main = new Main() )
         {
            main.ShowDialog();
         }
      }
   }
}
