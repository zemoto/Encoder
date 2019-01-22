using System.ComponentModel;

namespace Encoder.Operations
{
   internal enum OperationType
   {
      [Description( "Apply Filters" )]
      Filters,
      [Description( "Separate Streams" )]
      Separate
   }
}
