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
            var fileFormat = ( (EncodeVideoFilterViewModel)ViewModel ).FileFormat;
            return $"-c:v {fileFormat.GetAttribute<FilterEnumValueAttribute>().ParameterValue}";
         }
      }
   }
}
