using Encoder.UI;
using System.ComponentModel;

namespace Encoder.Filters.Video.Crop
{
   internal enum CropType
   {
      [Description( "Remove Black Bars" )]
      RemoveBlackBars,
      [Description( "Custom Crop" )]
      Custom
   }

   internal sealed class CropVideoFilterViewModel : FilterViewModel
   {
      private CropType _type;
      [FilterPropertyDescription( "Crop Type", "" )]
      public CropType Type
      {
         get => _type;
         set => SetProperty( ref _type, value );
      }

      private int _left;
      [FilterPropertyDescription( "Left", "" )]
      [PropertyDependency( nameof( Type ), CropType.Custom )]
      [PropertyMinMax( 0, 10000 )]
      public int Left
      {
         get => _left;
         set => SetProperty( ref _left, value );
      }

      private int _top;
      [FilterPropertyDescription( "Top", "" )]
      [PropertyDependency( nameof( Type ), CropType.Custom )]
      [PropertyMinMax( 0, 10000 )]
      public int Top
      {
         get => _top;
         set => SetProperty( ref _top, value );
      }

      private int _right;
      [FilterPropertyDescription( "Right", "" )]
      [PropertyDependency( nameof( Type ), CropType.Custom )]
      [PropertyMinMax( 0, 10000 )]
      public int Right
      {
         get => _right;
         set => SetProperty( ref _right, value );
      }

      private int _bottom;
      [FilterPropertyDescription( "Bottom", "" )]
      [PropertyDependency( nameof( Type ), CropType.Custom )]
      [PropertyMinMax( 0, 10000 )]
      public int Bottom
      {
         get => _bottom;
         set => SetProperty( ref _bottom, value );
      }
   }
}
