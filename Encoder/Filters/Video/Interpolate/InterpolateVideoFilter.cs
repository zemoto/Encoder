using System;

namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateVideoFilterViewModel();
      public override string FilterName { get; } = "Interpolate";
      private InterpolateVideoFilterViewModel InterpolateModel => (InterpolateVideoFilterViewModel)ViewModel;

      public override bool CanApplyFilter() => Math.Abs( InterpolateModel.TargetFrameRate - SourceMetadata.FrameRate ) > 0.001;

      protected override void InitializeEx()
      {
         TargetTotalFrames = (int)Math.Ceiling( SourceMetadata.Duration.TotalSeconds * InterpolateModel.TargetFrameRate );
         TargetBitrate = (uint)( 0.75 * SourceMetadata.Bitrate * InterpolateModel.TargetFrameRate / SourceMetadata.FrameRate );
      }
   }
}