using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZemotoUtils;

namespace Encoder.Encoding.Tasks
{
   internal abstract class EncodingTask : EncodingTaskBase
   {
      private bool _initialized;

      public override void Dispose() => CancelToken?.Dispose();

      public override void Cleanup() => FileUtils.SafeDeleteFile( TargetFile );

      public override void Cancel() => CancelToken.Cancel();

      public override string GetFilePath() => TargetFile;

      public bool Initialize( string directory, int id = -1 )
      {
         Debug.Assert( FileProvider != null );
         var sourceMetadata = VideoMetadataReader.GetVideoMetadata( SourceFile );
         if ( sourceMetadata == null )
         {
            Error = $"ffprobe could not read file: {SourceFile}";
            return false;
         }

         SourceMetadata = sourceMetadata;

         TargetFileExtension = Path.GetExtension( SourceFile ).TrimStart( '.' );
         TargetTotalFrames = (int)Math.Ceiling( SourceMetadata.FrameRate * SourceMetadata.Duration.TotalSeconds );

         InitializeEx();

         var targetFileName = id == -1 ? $"{Path.GetFileNameWithoutExtension( SourceFile )}.{TargetFileExtension}" :
                                         $"{Path.GetFileNameWithoutExtension( SourceFile )}-{id}.{TargetFileExtension}";
         TargetFile = Path.Combine( directory, targetFileName );

         if ( File.Exists( TargetFile ) )
         {
            TargetFile = string.Empty; // To avoid deleting
            Error = $"\"{targetFileName}\" exists in \"{directory}\"";
            return false;
         }

         _initialized = true;
         return true;
      }

      protected virtual void InitializeEx() { }

      public override bool DoWork()
      {
         Error = string.Empty;
         Started = true;
         if ( CancelToken.IsCancellationRequested )
         {
            return false;
         }

         var targetDirectory = Path.Combine( Path.GetDirectoryName( SourceFile ), "done" );
         UtilityMethods.CreateDirectory( targetDirectory );

         if ( !_initialized && !Initialize( targetDirectory ) )
         {
            if ( string.IsNullOrEmpty( Error ) )
            {
               Error = $"Could not fully process video file: {SourceFile}";
            }

            return false;
         }

         using ( var encoder = new FfmpegEncoder( this ) )
         {
            encoder.StartEncoding( CancelToken.Token );

            encoder.AwaitCompletion();

            if ( CancelToken.IsCancellationRequested )
            {
               Task.Delay( 300 );
               FileUtils.SafeDeleteFile( TargetFile );
            }

            if ( !string.IsNullOrEmpty( encoder.Error ) )
            {
               Error = $"ffmpeg Error: {encoder.Error}";
            }
         }

         return string.IsNullOrEmpty( Error ) && !CancelToken.IsCancellationRequested;
      }

      public abstract string DetailedTaskName { get; }

      public CancellationTokenSource CancelToken { get; } = new CancellationTokenSource();
      public abstract string EncodingArgs { get; }
      public string TargetFileExtension { get; protected set; }
      public string TargetFile { get; private set; }
   }
}
