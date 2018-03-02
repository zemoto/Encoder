using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
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

      private int _framesDone;
      private Bitmap _currentFrame;

      public Main()
      {
         _model = new MainViewModel
         {
            SelectVideoCommand = new RelayCommand( SelectVideo ),
            DoInterpolationCommand = new RelayCommand( DoInterpolation, () => _videoReader != null )
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
         try
         {
            _videoReader = new VideoFileReader();
            _videoReader.Open( videoFilePath );
            _currentFrame = _videoReader.ReadVideoFrame();

            _videoWriter = new VideoFileWriter();
            var fileName = Path.GetFileNameWithoutExtension( videoFilePath );
            var fileNameInterpolated = fileName + "-doubledfps";
            videoFilePath = videoFilePath.Replace( fileName, fileNameInterpolated );
            _videoWriter.Open( videoFilePath, _videoReader.Width, _videoReader.Height, _videoReader.FrameRate * 2, VideoCodec.H264, _videoReader.BitRate * 2 );
         }
         catch { /*file invalid*/ }
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
         try
         {
            for ( int i = 0; i < _videoReader.FrameCount; i++ )
            {
               await Task.Run( () =>
               {
                  WriteFrame( _currentFrame );
                  var first = _currentFrame;
                  var second = _currentFrame = _videoReader.ReadVideoFrame();
                  var interpolated = InterpolateFrames( first, second );
                  WriteFrame( interpolated );
               } );
            }
            _videoWriter.Flush();
            _videoWriter.Close();
         }
         catch ( Exception ex )
         {
            MessageBox.Show( ex.Message );
         }
      }

      private void WriteFrame( Bitmap frame )
      {
         if ( frame != null )
         {
            _videoWriter.WriteVideoFrame( frame );
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
            var interpolator = new BitmapInterpolator( first, second );
            var interpolatedFrame = interpolator.GetInterpolatedBitmap();
            _model.PercentDone = Math.Round( ++_framesDone / (double)_videoReader.FrameCount * 100, 2 );
            return interpolatedFrame;
         }
         catch
         {
            return null;
         }
      }
   }
}
