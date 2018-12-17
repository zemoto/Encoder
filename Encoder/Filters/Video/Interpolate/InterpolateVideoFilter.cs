using System;

namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateVideoFilter : VideoFilter
   {
      public override VideoFilterViewModel ViewModel { get; } = new InterpolateVideoFilterViewModel();
      public override string FilterName { get; } = "Interpolate";

      public override bool ShouldApplyFilter() => Math.Abs( ( (InterpolateVideoFilterViewModel)ViewModel ).TargetFrameRate - SourceFrameRate ) > 0.001;

      public override int GetTargetFrameCount() => (int)( SourceDuration.TotalSeconds * ( (InterpolateVideoFilterViewModel)ViewModel ).TargetFrameRate );
   }
}
