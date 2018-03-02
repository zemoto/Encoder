using System.Drawing;
using Accord.Video.FFMPEG;
using VideoInterpolator.Utils;

namespace VideoInterpolator
{
   internal sealed class Main
   {
      private readonly MainWindow _window;
      private readonly MainViewModel _model;

      private VideoFileReader _videoReader;

      public Main()
      {
         _model = new MainViewModel
         {
            SelectVideoCommand = new RelayCommand( SelectVideo ),
            NextFrameCommand = new RelayCommand( NextFrame, () => _videoReader != null )
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
         if ( !SelectFile( out string videoFilePath ) )
         {
            return;
         }

         try
         {
            _videoReader = new VideoFileReader();
            _videoReader.Open( videoFilePath );
            Bitmap frame;
            do
            {
               frame = _videoReader.ReadVideoFrame();
            } while ( frame == null );

            _model.Frame = frame;
         }
         catch
         {
            // file invalid
         }
      }

      private static bool SelectFile( out string filePath )
      {
         var dlg = new Microsoft.Win32.OpenFileDialog
         {
            DefaultExt = ".mp4",
            Filter = "MP4 Files (*.mp4)|*.mp4"
         };

         var result = dlg.ShowDialog();
         if ( result == true )
         {
            filePath = dlg.FileName;
            return true;
         }

         filePath = string.Empty;
         return false;
      }

      private void NextFrame()
      {
         _model.Frame = _videoReader.ReadVideoFrame();
      }
   }
}
