using System;
using System.Linq;
using System.Collections.Generic;
using Encoder.Encoding.Tasks;
using Encoder.Operations.Separate;

namespace Encoder.Operations
{
   internal abstract class Operation
   {
      private int _latestStep = 1;
      private readonly Dictionary<int,List<EncodingParams>> _steps = new Dictionary<int,List<EncodingParams>>();

      public MultiStepTask ToMultiStepTask( string file ) => new MultiStepTask( _steps.SelectMany( step => step.Value.Select( encodingParams => new TaskStep( step.Key, new EncodeWithCustomParams( file, encodingParams ) ) ).ToList() ).ToList() );

      protected void AddEncodingTask( EncodingParams encodingParams, bool addToLatestStep = false )
      {
         if ( !addToLatestStep )
         {
            _latestStep = Math.Max( 1, _latestStep + 1 );
         }

         if ( !_steps.ContainsKey( _latestStep ) )
         {
            _steps[_latestStep] = new List<EncodingParams>();
         }

         _steps[_latestStep].Add( encodingParams );
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
