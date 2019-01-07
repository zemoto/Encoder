namespace Encoder.Filters.Video.Amplify
{
   [Filter( "amplify" )]
   internal sealed class AmplifyVideoFilterViewModel : FilterViewModel
   {
      private int _radius = 2;
      [FilterPropertyDescription( "Frame Radius", "radius", 1, 63 )]
      public int Radius
      {
         get => _radius;
         set => SetProperty( ref _radius, value );
      }

      private int _factor = 2;
      [FilterPropertyDescription( "Amplify Factor", "factor", 0, 65535 )]
      public int Factor
      {
         get => _factor;
         set => SetProperty( ref _factor, value );
      }

      private int _threshold = 10;
      [FilterPropertyDescription( "Difference Threshold", "threshold", 0, 65535 )]
      public int Threshold
      {
         get => _threshold;
         set => SetProperty( ref _threshold, value );
      }

      private int _low = 65535;
      [FilterPropertyDescription( "Pixel Change Lower Limit", "low", 0, 65535 )]
      public int Low
      {
         get => _low;
         set => SetProperty( ref _low, value );
      }

      private int _high = 65535;
      [FilterPropertyDescription( "Pixel Change Upper Limit", "high", 0, 65535 )]
      public int High
      {
         get => _high;
         set => SetProperty( ref _high, value );
      }
   }
}
