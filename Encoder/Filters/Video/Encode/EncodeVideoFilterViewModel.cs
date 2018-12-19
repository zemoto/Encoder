namespace Encoder.Filters.Video.Encode
{
   internal enum VideoFileFormat
   {
      [FilterParameterValue( "libx264" )]
      MP4
   }

   [Filter( "encode", typeof( EncodeVideoFilterControl ) )]
   internal sealed class EncodeVideoFilterViewModel : VideoFilterViewModel
   {
      private VideoFileFormat _fileFormat = VideoFileFormat.MP4;
      public VideoFileFormat FileFormat
      {
         get => _fileFormat;
         set => SetProperty( ref _fileFormat, value );
      }
   }
}
