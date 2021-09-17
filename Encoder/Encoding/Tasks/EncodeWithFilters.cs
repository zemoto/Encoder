using Encoder.Filters;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;

namespace Encoder.Encoding.Tasks
{
   internal sealed class EncodeWithFilters : EncodingTask
   {
      private readonly VideoFilter _videoFilter;
      private readonly AudioFilter _audioFilter;

      private readonly bool _overrideBitrate;
      private readonly uint _customBitrate;

      public EncodeWithFilters( VideoFilter videoFilter, AudioFilter audioFilter, bool overrideBitrate, uint customBitrate )
      {
         _videoFilter = videoFilter;
         _audioFilter = audioFilter;
         _overrideBitrate = overrideBitrate;
         _customBitrate = customBitrate;
      }

      protected override void InitializeEx()
      {
         _videoFilter.Initialize( SourceFile, SourceMetadata );

         if ( !string.IsNullOrEmpty( _videoFilter.TargetExtension ) )
         {
            TargetFileExtension = _videoFilter.TargetExtension;
         }

         if ( _overrideBitrate )
         {
            _videoFilter.SetTargetBitrate( _customBitrate );
         }

         TargetTotalFrames = _videoFilter.TargetTotalFrames;
      }

      public override string EncodingArgs => $"-b:v {_videoFilter.TargetBitrate} {FilterArgumentBuilder.GetFilterArguments( _videoFilter )} {FilterArgumentBuilder.GetFilterArguments( _audioFilter )}";

      public override string TaskName => $"V:{_videoFilter.FilterName}, A:{_audioFilter.FilterName}";
      public override string DetailedTaskName => $"Filters - {TaskName}";
   }
}
