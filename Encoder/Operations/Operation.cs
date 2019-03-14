using System.Linq;
using System.Collections.Generic;
using Encoder.Encoding.Tasks;
using Encoder.Operations.Separate;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      protected List<OperationStep> Steps { get; } = new List<OperationStep>();

      public MultiStepTask ToMultiStepTask( string file ) => new MultiStepTask( Steps.Select( x => new TaskStep( x.Step, new EncodeWithCustomParams( file, x.Params ) ) ).ToList() );

      public static Operation GetOperationForType( OperationType type )
      {
         switch ( type )
         {
            case OperationType.Separate:
               return new SeparateOperation();
            default:
               return null;
         }
      }
   }
}
