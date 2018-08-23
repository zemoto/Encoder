using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Interpolator.Encoding;
using Interpolator.Utils;
using Microsoft.Win32;

namespace Interpolator.TaskCreation
{
   internal sealed class TaskCreationWizard
   {
      private TaskCreationWindow _window;
      private TaskCreationViewModel _model;

      public IEnumerable<EncodingTaskViewModel> CreateEncodingTasks()
      {
         _model = new TaskCreationViewModel
         {
            SelectFilesCommand = new RelayCommand( SelectFiles ),
            RemoveFileCommand = new RelayCommand<string>( file => _model.SelectedFiles.Remove( file ) ),
            CreateTasksCommand = new RelayCommand( () => _window.DialogResult = true, () => _model.SelectedFiles.Any() )
         };

         _window = new TaskCreationWindow
         {
            DataContext = _model,
            Owner = Application.Current.MainWindow
         };

         if ( _window.ShowDialog() == true )
         {
            return _model.SelectedFiles.Select( file => new EncodingTaskViewModel( file, _model.Filter ) );
         }

         return new List<EncodingTaskViewModel>();
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
