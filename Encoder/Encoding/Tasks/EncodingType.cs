using System.ComponentModel;

namespace Encoder.Encoding.Tasks
{
   internal enum EncodingType
   {
      [Description( "Apply Filters" )]
      Filters,
      [Description( "Apply Custom Params" )]
      Custom
   }
}
