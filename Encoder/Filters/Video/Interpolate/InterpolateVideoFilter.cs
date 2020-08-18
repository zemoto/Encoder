using System;
using Encoder.Encoding;

namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateVideoFilterViewModel();
      public override string FilterName { get; } = "Interpolate";
      private InterpolateVideoFilterViewModel InterpolateModel => (InterpolateVideoFilterViewModel)ViewModel;

      public override bool CanApplyFilter() => Math.Abs( InterpolateModel.TargetFrameRate - SourceMetadata.FrameRate ) > 0.001;

      public override void Initialize( string file, VideoMetadata metadata )
      {
         base.Initialize( file, metadata );
         TargetTotalFrames = (int)Math.Ceiling( metadata.Duration.TotalSeconds * InterpolateModel.TargetFrameRate );
         TargetBitrate = (uint)( 0.75 * metadata.Bitrate * InterpolateModel.TargetFrameRate / metadata.FrameRate );
      }
   }
}