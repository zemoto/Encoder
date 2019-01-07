using System.Collections.Generic;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      public List<OperationStep> Steps { get; } = new List<OperationStep>();
   }
}
