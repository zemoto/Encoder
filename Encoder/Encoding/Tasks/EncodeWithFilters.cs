using Encoder.Filters;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithFilters : SingleStepTask
   {
      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      public EncodeWithFilters( string sourceFile, VideoFilter videoFilter, AudioFilter audioFilter ) 
         : base( sourceFile )
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
      }

      public override bool Initialize()
      {
         if ( base.Initialize() )
         {
            _videoFilter.Initialize( SourceFrameRate, SourceDuration );
            TargetTotalFrames = _videoFilter.TargetTotalFrames;
            return true;
         }

         return false;
      }

      public override string EncodingArgs =>
         $"{FilterArgumentBuilder.GetFilterArguments( _videoFilter )} {FilterArgumentBuilder.GetFilterArguments( _audioFilter )}";

      public override string TargetFileExtension => "mp4";
      public override string TaskName => _videoFilter.FilterName;
   }
}
