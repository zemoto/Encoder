using System;

namespace Interpolator.Filters.Interpolate
{
   internal sealed class InterpolateFilter : Filter
   {
      public override FilterViewModel ViewModel { get; } = new InterpolateFilterViewModel();

      public override void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         base.Initialize( sourceFrameRate, sourceDuration );

         var vm = (InterpolateFilterViewModel)ViewModel;

         // Target the closest framerate that is multiple of the original framerate.
         // This should help mitigate artifacting.
         vm.TargetFrameRate = sourceFrameRate * Math.Floor( vm.TargetFrameRate / sourceFrameRate );
      }

      public override string GetFilterName() => "Interpolate";

      public override bool ShouldApplyFilter() => ( (InterpolateFilterViewModel)ViewModel ).TargetFrameRate / SourceFrameRate > 1.5;

      public override int GetTargetFrameCount() => (int)( SourceDuration.TotalSeconds * ( (InterpolateFilterViewModel)ViewModel ).TargetFrameRate );

      public override string GetFilterArguments()
      {
         var vm = (InterpolateFilterViewModel)ViewModel;
         return $"-filter:v \"minterpolate='fps={vm.TargetFrameRate}:mi_mode=mci:mc_mode=aobmc:me_mode=bidir'\"";
      }
   }
}
