using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      public abstract string Name { get; }

      public abstract EncodingTask CreateEncodingTask();
   }
}
