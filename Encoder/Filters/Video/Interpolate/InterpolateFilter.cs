using System;

namespace Encoder.Filters.Video.Interpolate
{
   internal sealed class InterpolateFilter : Filter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateFilterViewModel();
      public override string FilterName { get; } = "Interpolate";

      public override void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         base.Initialize( sourceFrameRate, sourceDuration );

         var vm = (InterpolateFilterViewModel)ViewModel;

         // Target the closest framerate that is multiple of the original framerate.
         // This should help mitigate artifacting.
         vm.TargetFrameRate = sourceFrameRate * Math.Floor( vm.TargetFrameRate / sourceFrameRate );
      }

      public override bool ShouldApplyFilter() => ( (InterpolateFilterViewModel)ViewModel ).TargetFrameRate / SourceFrameRate > 1.5;

      public override int GetTargetFrameCount() => (int)( SourceDuration.TotalSeconds * ( (InterpolateFilterViewModel)ViewModel ).TargetFrameRate );
   }
}
