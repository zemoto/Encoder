using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Encoder.TaskCreation;
using Encoder.Encoding;
using Encoder.Encoding.Tasks;
using Microsoft.Win32;
using ZemotoCommon.UI;

namespace Encoder
{
   internal sealed class Main : IDisposable
   {
      private readonly MainWindowViewModel _model;
      private readonly EncodingManager _encodingManager;

      public Main()
      {
         _encodingManager = new EncodingManager();

         var creationViewModel = new TaskCreationViewModel
         {
            SelectFilesCommand = new RelayCommand( SelectFiles )
         };

         creationViewModel.RemoveFileCommand = new RelayCommand<string>( file => creationViewModel.SelectedFiles.Remove( file ) );
         creationViewModel.CreateTasksCommand = new RelayCommand( CreateAndStartNewTasks, creationViewModel.SelectedFiles.Any );

         _model = new MainWindowViewModel( _encodingManager.Model, creationViewModel )
         {
            CancelTaskCommand = new RelayCommand<EncodingTaskBase>( _encodingManager.CancelTask )
         };
      }

      public void Dispose()
      {
         _encodingManager?.Dispose();
      }

      public void Show()
      {
         var window = new MainWindow( _model );
         window.Closing += OnMainWindowClosing;
         window.Show();
      }

      private async void OnMainWindowClosing( object sender, CancelEventArgs e )
      {
         if ( !_encodingManager.Model.AnyTasksPending )
         {
            return;
         }

         var result = MessageBox.Show( "Files are still being encoded, wait for cancellation and cleanup?", "Exiting", MessageBoxButton.YesNoCancel );
         if ( result == MessageBoxResult.Cancel )
         {
            e.Cancel = true;
         }
         else if ( result == MessageBoxResult.Yes )
         {
            e.Cancel = true;
            foreach ( var task in _encodingManager.Model.Tasks )
            {
               task.Cancel();
            }
            while ( _encodingManager.Model.AnyTasksPending )
            {
               await Task.Delay( 300 );
            }

            ( (Window)sender ).Close();
         }
      }

      private void CreateAndStartNewTasks()
      {
         var encodingTasks = _model.TaskCreationVm.GetEncodingTasks();
         _encodingManager.EnqueueTasks( encodingTasks.ToList() );
      }

      private void SelectFiles()
      {
         var dlg = new OpenFileDialog
         {
            Filter = "Video Files (*.mp4;*.wmv;*.webm;*.swf;*.mkv;*.avi)|*.mp4;*.wmv;*.webm;*.swf;*.mkv;*.avi|All files (*.*)|*.*",
            Multiselect = true
         };

         if ( dlg.ShowDialog( Application.Current.MainWindow ) == true )
         {
            foreach( var file in dlg.FileNames )
            {
               if ( !_model.TaskCreationVm.SelectedFiles.Contains( file ) )
               {
                  _model.TaskCreationVm.SelectedFiles.Add( file );
               }
            }
         }
      }
   }
}
