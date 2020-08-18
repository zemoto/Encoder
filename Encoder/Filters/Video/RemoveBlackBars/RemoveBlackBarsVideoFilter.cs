using Encoder.Encoding;

namespace Encoder.Filters.Video.RemoveBlackBars
{
   internal sealed class RemoveBlackBarsVideoFilter : VideoFilter
   {
      private CropDetect _crop;

      public override string FilterName { get; } = "Remove Black Bars";
      public override string CustomFilterArguments => _crop == null ? "-c:v copy" : $"-vf crop={_crop.Width}:{_crop.Height}:{_crop.X}:{_crop.Y}";

      public override void Initialize( string file, VideoMetadata metadata )
      {
         base.Initialize( file, metadata );

         _crop = VideoMetadataReader.GetCropDetect( file, metadata.Duration );
      }
   }
}
