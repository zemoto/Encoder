namespace Encoder.Filters.Video.Denoise
{
   [Filter( "atadenoise", typeof( DenoiseVideoFilterControl ) )]
   internal sealed class DenoiseVideoFilterViewModel : VideoFilterViewModel
   {
      private double _zeroA = 0.02;
      [FilterParameterName( "0a" )]
      public double ZeroA
      {
         get => _zeroA;
         set => SetProperty( ref _zeroA, value );
      }

      private double _zeroB = 0.04;
      [FilterParameterName( "0b" )]
      public double ZeroB
      {
         get => _zeroB;
         set => SetProperty( ref _zeroB, value );
      }

      private double _oneA = 0.02;
      [FilterParameterName( "1a" )]
      public double OneA
      {
         get => _oneA;
         set => SetProperty( ref _oneA, value );
      }

      private double _oneB = 0.04f;
      [FilterParameterName( "1b" )]
      public double OneB
      {
         get => _oneB;
         set => SetProperty( ref _oneB, value );
      }

      private double _twoA = 0.02;
      [FilterParameterName( "2a" )]
      public double TwoA
      {
         get => _twoA;
         set => SetProperty( ref _twoA, value );
      }

      private double _twoB = 0.04;
      [FilterParameterName( "2b" )]
      public double TwoB
      {
         get => _twoB;
         set => SetProperty( ref _twoB, value );
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
            SetProperty( ref _framesToAverage, value );
         }
      }
   }
}
