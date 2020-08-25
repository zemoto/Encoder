using Encoder.Encoding;
using System;

namespace Encoder.Filters.Video.Crop
{
   internal sealed class CropVideoFilter : VideoFilter
   {
      private CropDetect _crop;

      public override FilterViewModel ViewModel { get; } = new CropVideoFilterViewModel();
      public override string FilterName { get; } = "Crop";
      public override string CustomFilterArguments
      {
         get
         {
            var model = (CropVideoFilterViewModel)ViewModel;
            switch ( model.Type )
            {
               case CropType.RemoveBlackBars:
               {
                  if ( _crop != null )
                  {
                     return $"-vf crop={_crop.Width}:{_crop.Height}:{_crop.X}:{_crop.Y}";
                  }
                  break;
               }
               case CropType.Custom:
               {
                  return $"-vf crop=in_w-{model.Right + model.Left}:in_h-{model.Bottom + model.Top}:{model.Left}:{model.Top}";
               }
            }

            return "-c:v copy";
         }
      }

      protected override void InitializeEx()
      {
         var model = (CropVideoFilterViewModel)ViewModel;
         if ( model.Type == CropType.RemoveBlackBars )
         {
            _crop = VideoMetadataReader.GetCropDetect( SourceFile, SourceMetadata.Duration );
         }
      }
   }
}
