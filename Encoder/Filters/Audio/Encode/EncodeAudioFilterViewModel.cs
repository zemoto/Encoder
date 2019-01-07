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
      [FilterEnumValue( "OPUS", "libopus" )]
      Opus,
      [FilterEnumValue( "MP3", "libmp3lame" )]
      Mp3,
      [FilterEnumValue( "MP2", "libtwolame" )]
      Mp2,
      [FilterEnumValue( "AMR-NB", "libopencore-amrnb" )]
      AmrNb,
      [FilterEnumValue( "AMR-WB", "libvo-amrwbenc" )]
      AmrWb,
      [FilterEnumValue( "Ogg Vorbis", "libvorbis" )]
      OggVorbis,
      [FilterEnumValue( "WavPack", "wavpack" )]
      WavPack,
   }

   [Filter]
   internal sealed class EncodeAudioFilterViewModel : FilterViewModel
   {
      private AudioCodec _codec = AudioCodec.Aac;
      [FilterPropertyDescription( "Audio Codec", "" )]
      public AudioCodec Codec
      {
         get => _codec;
         set => SetProperty( ref _codec, value );
      }
   }
}
