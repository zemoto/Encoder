namespace Encoder.Filters.Video.Blur
{
   [Filter( "avgblur", typeof( BlurFilterControl ) )]
   internal sealed class BlurFilterViewModel : FilterViewModel
   {
      private int _horizontalRadius = 3;
      [FilterParameterName( "sizeX" )]
      public int HorizontalRadius
      {
         get => _horizontalRadius;
         set => SetProperty( ref _horizontalRadius, value );
      }

      private int _verticalRadius = 3;
      [FilterParameterName( "sizeY" )]
      public int VerticalRadius
      {
         get => _verticalRadius;
         set => SetProperty( ref _verticalRadius, value );
      }
   }
}
