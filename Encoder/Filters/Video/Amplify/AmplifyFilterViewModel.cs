namespace Encoder.Filters.Video.Amplify
{
   [Filter( "amplify", typeof( AmplifyFilterControl ) )]
   internal sealed class AmplifyFilterViewModel : FilterViewModel
   {
      private int _radius = 2;
      [FilterParameterName( "radius" )]
      public int Radius
      {
         get => _radius;
         set => SetClampedProperty( ref _radius, value, 1, 63 );
      }

      private int _factor = 2;
      [FilterParameterName( "factor" )]
      public int Factor
      {
         get => _factor;
         set => SetClampedProperty( ref _factor, value, 0, 65535 );
      }

      private int _threshold = 10;
      [FilterParameterName( "threshold" )]
      public int Threshold
      {
         get => _threshold;
         set => SetClampedProperty( ref _threshold, value, 0, 65525 );
      }

      private int _low = 65535;
      [FilterParameterName( "low" )]
      public int Low
      {
         get => _low;
         set => SetClampedProperty( ref _low, value, 0, 65535 );
      }

      private int _high = 65535;
      [FilterParameterName( "high" )]
      public int High
      {
         get => _high;
         set => SetClampedProperty( ref _high, value, 0, 65535 );
      }
   }
}
