using System.Collections.Generic;
using System.Linq;
using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      public IEnumerable<EncodingTaskBase> GetEncodingTasks( string file ) => CreateOperationChains( file ).Select( x => ConvertToAssemblyLineIfNeeded( x, file ) ).ToList();

      private static EncodingTaskBase ConvertToAssemblyLineIfNeeded( EncodingTask[] operationChain, string file )
      {
         var filePathProvider = new FilePathProvider( file );
         if ( operationChain.Length == 1 )
         {
            // Only a single task so we don't need an assembly line. Just return the task.
            var task = operationChain.First();
            task.SourceFilePathProvider = filePathProvider;
            return task;
         }

         return new AssemblyLine( filePathProvider, operationChain );
      }

      public AsyncOperation ToAsync() => new AsyncOperation( this );

      public abstract List<EncodingTask[]> CreateOperationChains( string file );
   }
}
