using Encoder.Filters;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithFilters : EncodingTask
   {
      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      public EncodeWithFilters( VideoFilter videoFilter, AudioFilter audioFilter ) 
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
      }

      public override bool Initialize( string directory, int id = -1 )
      {
         TargetFileExtension = _videoFilter.GetFilterTargetExtension( TargetFileExtension );

         if ( !base.Initialize( directory, id ) )
         {
            return false;
         }
         
         _videoFilter.Initialize( SourceFile, SourceFrameRate, SourceDuration );
         TargetTotalFrames = _videoFilter.TargetTotalFrames;
         TargetBitRate = _videoFilter.GetFilterTargetBitRate( TargetBitRate );
         return true;
      }

      public override string EncodingArgs => $"{FilterArgumentBuilder.GetFilterArguments( _videoFilter )} {FilterArgumentBuilder.GetFilterArguments( _audioFilter )}";

      public override string TaskName => $"V:{_videoFilter.FilterName}, A:{_audioFilter.FilterName}";
      public override string DetailedTaskName => $"Filters - {TaskName}";
   }
}
