using System;

namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateVideoFilterViewModel();
      public override string FilterName { get; } = "Interpolate";

      public override bool CanApplyFilter() => Math.Abs( ( (InterpolateVideoFilterViewModel)ViewModel ).TargetFrameRate - SourceFrameRate ) > 0.001;

      public override int GetTargetFrameCount() => (int)( SourceDuration.TotalSeconds * ( (InterpolateVideoFilterViewModel)ViewModel ).TargetFrameRate );
   }
}
