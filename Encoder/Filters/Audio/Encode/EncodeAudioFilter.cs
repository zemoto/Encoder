using ZemotoUtils;

namespace Encoder.Filters.Audio.Encode
{
   internal sealed class EncodeAudioFilter : AudioFilter
   {
      public override FilterViewModel ViewModel { get; } = new EncodeAudioFilterViewModel();
      public override string FilterName { get; } = "Encode";

      public override string CustomFilterArguments
      {
         get
         {
            var codec = ( (EncodeAudioFilterViewModel)ViewModel ).Codec;
            return $"-c:a {codec.GetAttribute<FilterEnumValueAttribute>().ParameterValue}";
         }
      }
   }
}
