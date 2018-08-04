namespace Interpolator.Filters.Interpolate
{
   internal sealed class InterpolateFilterViewModel : FilterViewModel
   {
      public double _targetFrameRate = 60;
      public double TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }
   }
}
