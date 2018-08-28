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
         set => SetProperty( ref _radius, value );
      }

      private int _factor = 2;
      [FilterParameterName( "factor" )]
      public int Factor
      {
         get => _factor;
         set => SetProperty( ref _factor, value );
      }

      private int _threshold = 10;
      [FilterParameterName( "threshold" )]
      public int Threshold
      {
         get => _threshold;
         set => SetProperty( ref _threshold, value );
      }

      private int _low = 65535;
      [FilterParameterName( "low" )]
      public int Low
      {
         get => _low;
         set => SetProperty( ref _low, value );
      }

      private int _high = 65535;
      [FilterParameterName( "high" )]
      public int High
      {
         get => _high;
         set => SetProperty( ref _high, value );
      }
   }
}
