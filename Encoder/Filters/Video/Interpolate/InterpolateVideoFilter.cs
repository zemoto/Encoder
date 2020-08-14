using System;

namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateVideoFilterViewModel();
      public override string FilterName { get; } = "Interpolate";
      private InterpolateVideoFilterViewModel InterpolateModel => (InterpolateVideoFilterViewModel)ViewModel;

      public override bool CanApplyFilter() => Math.Abs( InterpolateModel.TargetFrameRate - SourceFrameRate ) > 0.001;

      public override void Initialize( string file, double sourceFrameRate, TimeSpan sourceDuration )
      {
         base.Initialize( file, sourceFrameRate, sourceDuration );
         TargetTotalFrames = (int)Math.Ceiling( SourceDuration.TotalSeconds * InterpolateModel.TargetFrameRate );
      }

      public override uint GetFilterTargetBitRate( uint targetBitrateBeforeFilter )
      {
         return (uint)( 0.75 * targetBitrateBeforeFilter * InterpolateModel.TargetFrameRate / SourceFrameRate );
      }
   }
}