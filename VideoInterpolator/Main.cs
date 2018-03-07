using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using VideoInterpolator.Utils;

namespace VideoInterpolator
{
   internal sealed class Main
   {
      private readonly MainWindow _window;
      private readonly MainViewModel _model;

      private VideoFileReader _videoReader;
      private VideoFileWriter _videoWriter;

      private int _newFramesPerFrame;
      private int _partialFramesPerFrame;

      private const int TargetFrameRate = 60;

      public Main()
      {
         _model = new MainViewModel
         {
            SelectVideoCommand = new RelayCommand( SelectVideo ),
            DoInterpolationCommand = new RelayCommand( () => Task.Run( () => DoInterpolation() ), () => _videoReader != null )
         };

         _window = new MainWindow
         {
            DataContext = _model
         };
      }

      public void ShowDialog()
      {
         _window.ShowDialog();
      }

      private void SelectVideo()
      {
         if ( !SelectFile( out var videoFilePath ) )
         {
            return;
         }

         var fileName = Path.GetFileNameWithoutExtension( videoFilePath );
         var fileNameInterpolated = fileName + "-doubledfps";

         try
         {
            _videoReader = new VideoFileReader();
            _videoReader.Open( videoFilePath );
            _model.TotalFrames = _videoReader.FrameCount - 1;

            _newFramesPerFrame = (int)Math.Ceiling( TargetFrameRate / (double)_videoReader.FrameRate ) - 1;

            _videoWriter = new VideoFileWriter();
            videoFilePath = videoFilePath.Replace( fileName, fileNameInterpolated );
            _videoWriter.Open( videoFilePath, _videoReader.Width, _videoReader.Height, _videoReader.FrameRate * ( _newFramesPerFrame + 1 ), VideoCodec.H264, GetNewBitRate() );
         }
         catch { /*file invalid*/ }
      }

      private int GetNewBitRate()
      {
         var newFrameRate = (double)_videoReader.FrameRate * ( _newFramesPerFrame + 1 );
         return (int)( _videoReader.Width * _videoReader.Height * newFrameRate * 0.28 );
      }

      private static bool SelectFile( out string filePath )
      {
         var dlg = new Microsoft.Win32.OpenFileDialog
         {
            DefaultExt = ".mp4",
            Filter = "MP4 Files (*.mp4)|*.mp4"
         };
         var result = dlg.ShowDialog();
         filePath = dlg.FileName;
         return result == true;
      }

      private async void DoInterpolation()
      {
         _model.PercentDone = 0;

         var index = 0;
         var currentFrame = _videoReader.ReadVideoFrame();
         for ( int i = 0; i < _videoReader.FrameCount; i++ )
         {
            if ( currentFrame != null )
            {
               var first = (Bitmap)currentFrame.Clone();
               currentFrame = _videoReader.ReadVideoFrame();
               if ( currentFrame != null )
               {
                  var second = currentFrame;
                  InterpolateAndBufferFrames( first, second );
                  index += 2;
               }
            }
         }

         _videoWriter.Flush();
         _videoWriter.Close();
      }

      private void InterpolateAndBufferFrames( Bitmap first, Bitmap second )
      {
         var leftFrame = (Bitmap)first.Clone();
         WriteFrame( first );

         for ( int i = 0; i < _newFramesPerFrame; i++ )
         {
            var interpolated = InterpolateFrames( leftFrame, second );
            leftFrame = (Bitmap)interpolated.Clone();
            WriteFrame( interpolated );
         }

         _model.FramesDone += 1;
      }

      private void WriteFrame( Bitmap frame )
      {
         if ( frame != null )
         {
            _videoWriter.WriteVideoFrame( frame );
            frame.Dispose();
         }
      }

      private Bitmap InterpolateFrames( Bitmap first, Bitmap second )
      {
         if ( first == null || second == null )
         {
            return null;
         }
         try
         {
            var interpolatedFrame = BitmapInterpolator.InterpolateBitmaps( first, second );
            return interpolatedFrame;
         }
         catch
         {
            return null;
         }
      }
   }
}
