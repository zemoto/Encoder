namespace Encoder.Filters.Video.Encode
{
   internal enum VideoFileFormat
   {
      [FilterEnumValue( "libx264" )]
      H264,
      [FilterEnumValue( "libx265" )]
      H265
   }

   [Filter]
   internal sealed class EncodeVideoFilterViewModel : FilterViewModel
   {
      private VideoFileFormat _fileFormat = VideoFileFormat.H264;
      [FilterParameter( "File Format", "" )]
      public VideoFileFormat FileFormat
      {
         get => _fileFormat;
         set => SetProperty( ref _fileFormat, value );
      }
   }
}
