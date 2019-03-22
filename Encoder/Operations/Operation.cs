using System.Collections.Generic;
using System.Linq;
using Encoder.Encoding.Tasks;

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
         AddEncodingOperationChain( new[] { encodingTask } );
      }
   }
}
