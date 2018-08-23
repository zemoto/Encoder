using System.Linq;
using System.Windows;
using Interpolator.Encoding;
using Interpolator.Utils;
using Microsoft.Win32;

namespace Interpolator.JobCreation
{
   internal sealed class JobCreationWizard
   {
      private JobCreationWindow _window;
      private JobCreationViewModel _model;

      public EncodingJob CreateJob()
      {
         _model = new JobCreationViewModel
         {
            SelectFilesCommand = new RelayCommand( SelectFiles ),
            RemoveFileCommand = new RelayCommand<string>( file => _model.SelectedFiles.Remove( file ) ),
            CreateJobCommand = new RelayCommand( () => _window.DialogResult = true, () => _model.SelectedFiles.Any() )
         };

         _window = new JobCreationWindow
         {
            DataContext = _model,
            Owner = Application.Current.MainWindow
         };

         return _window.ShowDialog() == true ? new EncodingJob( _model.SelectedFiles, _model.Filter ) : null;
      }

      private void SelectFiles()
      {
         var dlg = new OpenFileDialog
         {
            Filter = "Video Files (*.mp4;*.wmv;*.webm;*.swf;*.mkv)|*.mp4;*.wmv;*.webm;*.swf;*.mkv|All files (*.*)|*.*",
            Multiselect = true
         };

         if ( dlg.ShowDialog( _window ) == true )
         {
            foreach( var file in dlg.FileNames )
            {
               if ( !_model.SelectedFiles.Contains( file ) )
               {
                  _model.SelectedFiles.Add( file );
               }
            }
         }
      }
   }
}
