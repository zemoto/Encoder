using System.Collections.Generic;
using System.Linq;
using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      public IEnumerable<AssemblyLine> GetAssemblyLines( string file )
      {
         var operations =  CreateOperationChains( file );
         return operations?.Select( x => new AssemblyLine( file, x ) ).ToList();
      }

      public AsyncOperation ToAsync() => new AsyncOperation( this );

      public abstract List<EncodingTask[]> CreateOperationChains( string file );
   }
}
