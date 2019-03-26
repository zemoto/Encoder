using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZemotoCommon.Utils;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTask : EncodingTaskBase
   {
      private bool _initialized;

      public override void Dispose() => CancelToken?.Dispose();

      public override void Cleanup() => UtilityMethods.SafeDeleteFile( TargetFile );

      public override void Cancel() => CancelToken.Cancel();

      public override string GetFilePath() => TargetFile;

      public virtual bool Initialize( string directory, int id = -1 )
      {
         Debug.Assert( FileProvider != null );
         bool success = VideoMetadataReader.GetVideoInfo( SourceFile, out var sourceFrameRate, out var sourceDuration );
         if ( !success )
         {
            Error = $"Could not fully process video file: {SourceFile}";
            return false;
         }

         SourceFrameRate = sourceFrameRate;
         SourceDuration = sourceDuration;
         TargetTotalFrames = (int)Math.Ceiling( SourceFrameRate * SourceDuration.TotalSeconds );

         var targetFileName = id == -1 ? $"{Path.GetFileNameWithoutExtension( SourceFile )}.{TargetFileExtension}" : 
                                         $"{Path.GetFileNameWithoutExtension( SourceFile )}-{id}.{TargetFileExtension}";
         TargetFile = Path.Combine( directory, targetFileName );

         _initialized = true;
         return true;
      }

      public override bool DoWork()
      {
         Started = true;
         if ( CancelToken.IsCancellationRequested )
         {
            return false;
         }

         if ( !_initialized && !Initialize( Path.Combine( Path.GetDirectoryName( SourceFile ), "done" ) ) )
         {
            Error = $"Could not fully process video file: {SourceFile}";
            return false;
         }

         using ( var encoder = new FfmpegEncoder( this ) )
         {
            encoder.StartEncoding( CancelToken.Token );

            encoder.AwaitCompletion();

            if ( CancelToken.IsCancellationRequested )
            {
               Task.Delay( 300 );
               UtilityMethods.SafeDeleteFile( TargetFile );
            }

            if ( !string.IsNullOrEmpty( encoder.Error ) )
            {
               Error = $"Error: {encoder.Error}";
            }
         }

         return string.IsNullOrEmpty( Error ) && !CancelToken.IsCancellationRequested;
      }

      public CancellationTokenSource CancelToken { get; } = new CancellationTokenSource();
      public abstract string EncodingArgs { get; }
      public abstract string TargetFileExtension { get; }
      public string TargetFile { get; private set; }
   }
}
