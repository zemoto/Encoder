namespace Encoder.Filters.Video.Denoise
{
   [Filter( "atadenoise", typeof( DenoiseFilterControl ) )]
   internal sealed class DenoiseFilterViewModel : FilterViewModel
   {
      private double _zeroA = 0.02;
      [FilterParameterName( "0a" )]
      public double ZeroA
      {
         get => _zeroA;
         set => SetClampedProperty( ref _zeroA, value, 0, 0.3 );
      }

      private double _zeroB = 0.04;
      [FilterParameterName( "0b" )]
      public double ZeroB
      {
         get => _zeroB;
         set => SetClampedProperty( ref _zeroB, value, 0, 5 );
      }

      private double _oneA = 0.02;
      [FilterParameterName( "1a" )]
      public double OneA
      {
         get => _oneA;
         set => SetClampedProperty( ref _oneA, value, 0, 0.3 );
      }

      private double _oneB = 0.04;
      [FilterParameterName( "1b" )]
      public double OneB
      {
         get => _oneB;
         set => SetClampedProperty( ref _oneB, value, 0, 5 );
      }

      private double _twoA = 0.02;
      [FilterParameterName( "2a" )]
      public double TwoA
      {
         get => _twoA;
         set => SetClampedProperty( ref _twoA, value, 0, 0.3 );
      }

      private double _twoB = 0.04;
      [FilterParameterName( "2b" )]
      public double TwoB
      {
         get => _twoB;
         set => SetClampedProperty( ref _twoB, value, 0, 5 );
      }

      private int _framesToAverage = 9;
      [FilterParameterName( "s" )]
      public int FramesToAverage
      {
         get => _framesToAverage;
         set
         {
            if ( value % 2 == 0 )
            {
               value -= 1; // Odd numbers only
            }
            SetClampedProperty( ref _framesToAverage, value, 5, 129 );
         }
      }
   }
}
