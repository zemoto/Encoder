namespace Encoder.Filters.Video.Blur
{
   [Filter( "avgblur" )]
   internal sealed class BlurVideoFilterViewModel : VideoFilterViewModel
   {
      private int _horizontalRadius = 3;
      [FilterParameter( "Horizontal Radius", "sizeX", 0, 10000 )]
      public int HorizontalRadius
      {
         get => _horizontalRadius;
         set => SetProperty( ref _horizontalRadius, value );
      }

      private int _verticalRadius = 3;
      [FilterParameter( "Vertical Radius", "sizeY", 0, 10000 )]
      public int VerticalRadius
      {
         get => _verticalRadius;
         set => SetProperty( ref _verticalRadius, value );
      }
   }
}
