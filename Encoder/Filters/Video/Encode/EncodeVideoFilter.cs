using ZemotoCommon.Utils;

namespace Encoder.Filters.Video.Encode
{
   internal sealed class EncodeVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new EncodeVideoFilterViewModel();
      public override string FilterName { get; } = "Encode";

      public override string CustomFilterArguments
      {
         get
         {
            var codec = ( (EncodeVideoFilterViewModel)ViewModel ).Codec;
            return $"-c:v {codec.GetAttribute<FilterEnumValueAttribute>().ParameterValue}";
         }
      }

      protected override void InitializeEx()
      {
         var model = ( (EncodeVideoFilterViewModel)ViewModel );
         TargetBitrate = model.UseCustomBitrate ? ( model.CustomBitrate * 1000 ) /*Model has kbps but backend wants bps*/ : SourceMetadata.Bitrate;
         TargetExtension = model.GetTargetExtension();
      }
   }
}
