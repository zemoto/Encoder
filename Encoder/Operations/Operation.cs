using System.Collections.Generic;
using System.Linq;
using Encoder.Encoding.Tasks;
using Encoder.Operations.Separate;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      private readonly List<EncodingTask[]> _steps = new List<EncodingTask[]>();

      public IEnumerable<AssemblyLine> ToAssemblyLines( string file ) => _steps.Select( x => new AssemblyLine( file, x ) );

      protected void AddEncodingOperationChain( EncodingTask[] encodingTasks )
      {
         _steps.Add( encodingTasks );
      }

      protected void AddEncodingOperation( EncodingTask encodingTask )
      {
         _steps.Add( new[] { encodingTask } );
      }

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
