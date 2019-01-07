using Encoder.UI;

namespace Encoder.Filters.Video.Blur
{
   [Filter( "avgblur" )]
   internal sealed class BlurVideoFilterViewModel : FilterViewModel
   {
      private int _horizontalRadius = 3;
      [FilterPropertyDescription( "Horizontal Radius", "sizeX" )]
      [PropertyMinMax( 0, 10000 )]
      public int HorizontalRadius
      {
         get => _horizontalRadius;
         set => SetProperty( ref _horizontalRadius, value );
      }

      private int _verticalRadius = 3;
      [FilterPropertyDescription( "Vertical Radius", "sizeY" )]
      [PropertyMinMax( 0, 10000 )]
      public int VerticalRadius
      {
         get => _verticalRadius;
         set => SetProperty( ref _verticalRadius, value );
      }
   }
}
