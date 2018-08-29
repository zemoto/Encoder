using System;
using Encoder.Filters.Video.Amplify;
using Encoder.Filters.Video.Blur;
using Encoder.Filters.Video.Denoise;
using Encoder.Filters.Video.Interpolate;

namespace Encoder.Filters
{
   internal abstract class Filter
   {
      protected double SourceFrameRate { get; private set; }
      protected TimeSpan SourceDuration { get; private set; }

      public abstract FilterViewModel ViewModel { get; }
      public abstract string FilterName { get; }

      public virtual void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
      }

      public virtual bool ShouldApplyFilter() => true;
      public virtual int GetTargetFrameCount() => (int)Math.Ceiling( SourceFrameRate * SourceDuration.TotalSeconds );
   }

   internal static class FilterProvider
   {
      public static Filter GetFilterForType( FilterType type )
      {
         switch ( type )
         {
            case FilterType.Amplify:
               return new AmplifyFilter();
            case FilterType.Blur:
               return new BlurFilter();
            case FilterType.Denoise:
               return new DenoiseFilter();
            case FilterType.Interpolate:
               return new InterpolateFilter();
            default:
               return null;
         }
      }
   }
}
