using System.Collections.Generic;
using System.Linq;
using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      public IEnumerable<EncodingTaskBase> GetEncodingTasks( string file )
      {
         var operationChains = CreateOperationChains( file );
         var tasks = new List<EncodingTaskBase>();
         for ( int i = 0; i < operationChains.Count; i++ )
         {
            tasks.Add( ConvertToAssemblyLineIfNeeded( operationChains[i], file, i ) );
         }
         return tasks;
      }

      private static EncodingTaskBase ConvertToAssemblyLineIfNeeded( EncodingTask[] operationChain, string file, int index )
      {
         var filePathProvider = new FilePathProvider( file );
         if ( operationChain.Length == 1 )
         {
            // Only a single task so we don't need an assembly line. Just return the task.
            var task = operationChain.First();
            task.FileProvider = filePathProvider;
            return task;
         }

         return new AssemblyLine( filePathProvider, operationChain, index );
      }

      public AsyncOperation ToAsync() => new AsyncOperation( this );

      public abstract List<EncodingTask[]> CreateOperationChains( string file );
   }
}
