using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Encoder.Encoding;
using Encoder.Encoding.Tasks;
using Encoder.ffmpeg;

namespace Encoder.Operations
{
   internal sealed class AsyncOperation : Operation
   {
      private const int SecondsBetweenSplits = 10;

      private readonly Operation _operation;

      public AsyncOperation( Operation operation )
      {
         Debug.Assert( !(operation is AsyncOperation) );
         _operation = operation;
      }

      public override List<EncodingTask[]> CreateOperationChains( string file )
      {
         if ( !VideoMetadataReader.GetKeyframes( file, out var keyFrames ) )
         {
            Debug.Assert( false );
            return null;
         }

         var operationChains = new List<EncodingTask[]>();
         double currentTime = 0;
         while ( currentTime + SecondsBetweenSplits < keyFrames.Last() )
         {
            var endKeyFrame = GetClosestKeyFrame( currentTime + SecondsBetweenSplits, keyFrames );
            operationChains.AddRange( AppendSplitOperationToChains( currentTime, endKeyFrame, file ) );

            currentTime = endKeyFrame;
         }

         // Last cut through to the end
         operationChains.AddRange( AppendSplitOperationToChains( currentTime, currentTime, file ) );

         return operationChains;
      }

      private IEnumerable<EncodingTask[]> AppendSplitOperationToChains( double startTime, double endTime, string file )
      {
         var startTimeString = startTime.ToTimeString();
         var durationString = ( endTime - startTime ).ToTimeString();

         EncodingParams encodingParams;
         if ( Math.Abs( startTime ) < 0.001 )
         {
            encodingParams = new EncodingParams( $"First cut to {endTime}", $"-t {durationString}", "mp4", true );
         }
         else if ( Math.Abs( startTime - endTime ) < 0.001 )
         {
            encodingParams = new EncodingParams( $"Final cut from {startTime}", $"-ss {startTimeString}", "mp4", true );
         }
         else
         {
            encodingParams = new EncodingParams( $"Cutting from {startTime} to {endTime}", $"-ss {startTimeString} -t {durationString}", "mp4", true );
         }

         var finalOperationChains = new List<EncodingTask[]>();
         foreach ( var operationChain in _operation.CreateOperationChains( file ).Select( x => x.ToList() ) )
         {
            operationChain.Insert( 0, new EncodeWithCustomParams( encodingParams ) );
            finalOperationChains.Add( operationChain.ToArray() );
         }

         return finalOperationChains;
      }

      private static double GetClosestKeyFrame( double target, IEnumerable<double> keyFrames )
      {
         double closest = 0.0;
         double distance = double.PositiveInfinity;
         foreach ( var keyFrame in keyFrames.Where( x => x >= target ) )
         {
            var newDistance = Math.Abs( target - keyFrame );
            if ( newDistance < distance )
            {
               distance = newDistance;
               closest = keyFrame;
            }
            else
            {
               break;
            }
         }

         return closest;
      }
   }
}
