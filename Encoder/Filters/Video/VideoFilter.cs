using Encoder.Encoding;
using System;

namespace Encoder.Filters.Video
{
   internal abstract class VideoFilter : Filter
   {
      protected string SourceFile { get; private set; }
      protected VideoMetadata SourceMetadata { get; private set; }
      public int TargetTotalFrames { get; protected set; }

      public virtual void Initialize( string file, VideoMetadata metadata )
      {
         SourceFile = file;
         SourceMetadata = metadata;
         TargetTotalFrames = (int)Math.Ceiling( SourceMetadata.FrameRate * SourceMetadata.Duration.TotalSeconds );
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
