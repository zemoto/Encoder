using System.Collections.Generic;
using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal sealed class SeparateOperation : Operation
   {
      public override List<EncodingTask[]> CreateOperationChains( string file )
      {
         return new List<EncodingTask[]>
         {
            new EncodingTask[]
            {
               new EncodeWithCustomParams( new EncodingParams( "Separate into video", "-c:v copy -an" ) ),
               new EncodeWithCustomParams( new EncodingParams( "Separate into audio", "-vn -c:a copy", "wmv" ) )
            }
         };
      }
   }
}
