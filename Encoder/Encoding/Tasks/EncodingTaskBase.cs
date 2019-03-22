using System;
using ZemotoCommon.UI;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTaskBase : ViewModelBase, IFilePathProvider
   {
      public event EventHandler<bool> TaskFinished;
      protected void RaiseTaskFinished( bool success )
      {
         TaskFinished?.Invoke( this, success );
      }

      public IFilePathProvider SourceFilePathProvider { get; set; }

      public abstract void Cleanup();
      public abstract void Cancel();
      public abstract string GetFilePath();
   }
}
