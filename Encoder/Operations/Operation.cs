using System.Collections.Generic;
using Encoder.Operations.Separate;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      public List<OperationStep> Steps { get; } = new List<OperationStep>();

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
