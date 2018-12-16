namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateFilter : Filter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateFilterViewModel();
      public override string FilterName { get; } = "Interpolate";

      public override bool ShouldApplyFilter() => ( (InterpolateFilterViewModel)ViewModel ).TargetFrameRate / SourceFrameRate > 1.5;

      public override int GetTargetFrameCount() => (int)( SourceDuration.TotalSeconds * ( (InterpolateFilterViewModel)ViewModel ).TargetFrameRate );
   }
}
