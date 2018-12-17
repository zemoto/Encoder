using System;
using Encoder.Filters.Video.Amplify;
using Encoder.Filters.Video.Blur;
using Encoder.Filters.Video.Denoise;
using Encoder.Filters.Video.Interpolate;

namespace Encoder.Filters.Video
{
   internal abstract class VideoFilter
   {
      protected double SourceFrameRate { get; private set; }
      protected TimeSpan SourceDuration { get; private set; }

      public abstract VideoFilterViewModel ViewModel { get; }
      public abstract string FilterName { get; }

      public virtual void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
      }

      public virtual bool ShouldApplyFilter() => true;
      public virtual int GetTargetFrameCount() => (int)Math.Ceiling( SourceFrameRate * SourceDuration.TotalSeconds );
   }

   internal static class VideoFilterProvider
   {
      public static VideoFilter GetFilterForType( VideoFilterType type )
      {
         switch ( type )
         {
            case VideoFilterType.Amplify:
               return new AmplifyVideoFilter();
            case VideoFilterType.Blur:
               return new BlurVideoFilter();
            case VideoFilterType.Denoise:
               return new DenoiseVideoFilter();
            case VideoFilterType.Interpolate:
               return new InterpolateVideoFilter();
            default:
               return null;
         }
      }
   }
}
