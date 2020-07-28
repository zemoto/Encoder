using System.Collections.Generic;
using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal sealed class CustomParamsOperation : Operation
   {
      private readonly string _customParams;
      private readonly string _extension;

      public CustomParamsOperation( string customParams, string extension )
      {
         _customParams = customParams;
         _extension = extension;
      }

      public override List<EncodingTask[]> CreateOperationChains( string file )
      {
         return new List<EncodingTask[]>
         {
            new EncodingTask[]
            {
               new EncodeWithCustomParams( new EncodingParams( "Custom", _customParams, _extension ) ),
            }
         };
      }
   }
}
