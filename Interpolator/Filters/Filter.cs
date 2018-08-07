using System;
using Interpolator.Filters.Interpolate;

namespace Interpolator.Filters
{
   internal abstract class Filter
   {
      protected double SourceFrameRate { get; private set; }
      protected TimeSpan SourceDuration { get; private set; }

      public virtual void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
      }

      public abstract bool ShouldApplyFilter();
      public abstract int GetTargetFrameCount();

      public abstract FilterViewModel ViewModel { get; }
      public abstract string FilterName { get; }
   }

   internal static class FilterProvider
   {
      public static Filter GetFilterForType( FilterType type )
      {
         switch ( type )
         {
            case FilterType.Interpolate:
               return new InterpolateFilter();
            default:
               return null;
         }
      }
   }
}
