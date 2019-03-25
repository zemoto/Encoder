using System.Collections.Generic;
using System.Linq;
using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      protected bool IsMultiStep;

      public IEnumerable<EncodingTaskBase> GetEncodingTasks( string file )
      {
         var operations = CreateOperationChains( file );
         if ( IsMultiStep )
         {
            return operations.Select( x => new AssemblyLine( new FilePathProvider( file ), x ) ).ToList();
         }

         var tasks = operations.SelectMany( x => x ).ToList();
         tasks.ForEach( x => x.SourceFilePathProvider = new FilePathProvider( file ) );
         return tasks;
      }

      public AsyncOperation ToAsync() => new AsyncOperation( this );

      public abstract List<EncodingTask[]> CreateOperationChains( string file );
   }
}
