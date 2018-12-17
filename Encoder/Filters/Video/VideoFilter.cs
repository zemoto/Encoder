using System;

namespace Encoder.Filters.Video
{
   internal abstract class VideoFilter
   {
      protected double SourceFrameRate { get; private set; }
      protected TimeSpan SourceDuration { get; private set; }

      public abstract VideoFilterViewModel ViewModel { get; }
      public abstract string FilterName { get; }
      public virtual string CustomFilterArguments { get; } = null;

      public virtual void Initialize( double sourceFrameRate, TimeSpan sourceDuration )
      {
         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
      }

      public virtual bool CanApplyFilter() => true;
      public virtual int GetTargetFrameCount() => (int)Math.Ceiling( SourceFrameRate * SourceDuration.TotalSeconds );
   }
}
