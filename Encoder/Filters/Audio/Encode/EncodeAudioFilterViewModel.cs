namespace Encoder.Filters.Audio.Encode
{
   internal enum AudioCodec
   {
      [FilterEnumValue( "AAC", "aac" )]
      Aac,
      [FilterEnumValue( "AC3", "ac3" )]
      Ac3,
      [FilterEnumValue( "FLAC", "flac" )]
      Flac,
      [FilterEnumValue( "OPUS", "opus" )]
      Opus
   }

   [Filter]
   internal sealed class EncodeAudioFilterViewModel : FilterViewModel
   {
      private AudioCodec _codec = AudioCodec.Aac;
      [FilterParameter( "Audio Codec", "" )]
      public AudioCodec Codec
      {
         get => _codec;
         set => SetProperty( ref _codec, value );
      }
   }
}
