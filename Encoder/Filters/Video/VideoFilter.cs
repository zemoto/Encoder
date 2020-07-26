using System;

namespace Encoder.Filters.Video
{
   internal abstract class VideoFilter : Filter
   {
      protected double SourceFrameRate { get; private set; }
      protected TimeSpan SourceDuration { get; private set; }
      public int TargetTotalFrames { get; protected set; }

      public virtual void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
         TargetTotalFrames = (int)Math.Ceiling( SourceFrameRate * SourceDuration.TotalSeconds );
      }

      public static VideoFilter GetFilterForType( VideoFilterType type )
      {
         var filterName = type.ToString();
         var filterType = Type.GetType( $"Encoder.Filters.Video.{filterName}.{filterName}VideoFilter" );
         if ( filterType != null )
         {
            return (VideoFilter)Activator.CreateInstance( filterType );
         }

         throw new NotImplementedException( "Filter not implemented, named incorrectly, or in wrong namespace" );
      }

      public virtual uint GetFilterTargetBitRate( uint targetBitrateBeforeFilter ) => targetBitrateBeforeFilter;

      public virtual string GetFilterTargetExtension( string currentTargetExtension ) => currentTargetExtension;
   }
}
