namespace Encoder.Filters.Video.Encode
{
   internal enum VideoCodec
   {
      [FilterEnumValue( "libx264" )]
      H264,
      [FilterEnumValue( "libx265" )]
      H265
   }

   [Filter]
   internal sealed class EncodeVideoFilterViewModel : FilterViewModel
   {
      private VideoCodec _codec = VideoCodec.H264;
      [FilterParameter( "Video Codec", "" )]
      public VideoCodec Codec
      {
         get => _codec;
         set => SetProperty( ref _codec, value );
      }
   }
}
