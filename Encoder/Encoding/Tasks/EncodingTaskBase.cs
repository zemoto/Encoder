using System;
using ZemotoCommon.UI;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTaskBase : ViewModelBase
   {
      public event EventHandler<bool> TaskFinished;
      protected void RaiseTaskFinished( bool success )
      {
         TaskFinished?.Invoke( this, success );
      }

      public abstract void Cleanup();
      public abstract void Cancel();
   }
}
