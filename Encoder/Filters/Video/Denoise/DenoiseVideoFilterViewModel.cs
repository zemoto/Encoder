using Encoder.UI;

namespace Encoder.Filters.Video.Denoise
{
   [Filter( "atadenoise" )]
   internal sealed class DenoiseVideoFilterViewModel : FilterViewModel
   {
      private double _zeroA = 0.02;
      [FilterPropertyDescription( "Threshold 0A", "0a" )]
      [PropertyMinMax( 0, 0.3 )]
      public double ZeroA
      {
         get => _zeroA;
         set => SetProperty( ref _zeroA, value );
      }

      private double _zeroB = 0.04;
      [FilterPropertyDescription( "Threshold 0B", "0b" )]
      [PropertyMinMax( 0, 5 )]
      public double ZeroB
      {
         get => _zeroB;
         set => SetProperty( ref _zeroB, value );
      }

      private double _oneA = 0.02;
      [FilterPropertyDescription( "Threshold 1A", "1a" )]
      [PropertyMinMax( 0, 0.3 )]
      public double OneA
      {
         get => _oneA;
         set => SetProperty( ref _oneA, value );
      }

      private double _oneB = 0.04;
      [FilterPropertyDescription( "Threshold 1B", "1b" )]
      [PropertyMinMax( 0, 5 )]
      public double OneB
      {
         get => _oneB;
         set => SetProperty( ref _oneB, value );
      }

      private double _twoA = 0.02;
      [FilterPropertyDescription( "Threshold 2A", "2a" )]
      [PropertyMinMax( 0, 0.3 )]
      public double TwoA
      {
         get => _twoA;
         set => SetProperty( ref _twoA, value );
      }

      private double _twoB = 0.04;
      [FilterPropertyDescription( "Threshold 2B", "2b" )]
      [PropertyMinMax( 0, 5 )]
      public double TwoB
      {
         get => _twoB;
         set => SetProperty( ref _twoB, value );
      }

      private int _framesToAverage = 9;
      [FilterPropertyDescription( "Frames To Average", "s" )]
      [PropertyMinMax( 5, 129 )]
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
