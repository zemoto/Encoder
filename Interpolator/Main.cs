using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Interpolator.Encoding;
using Microsoft.Win32;

namespace Interpolator
{
   internal sealed class Main
   {
      private MainWindowViewModel _model;

      private readonly object _taskLock = new object();

      public Main()
      {
         _model = new MainWindowViewModel
         {
            SelectFilesCommand = new RelayCommand( SelectFiles ),
            StartTaskCommand = new RelayCommand( StartEncodingTask, () => _model.SelectedFiles?.Any() == true )
         };
      }

      public void ShowDialog()
      {
         var window = new MainWindow( _model );
         window.Closing += OnMainWindowClosing;
         window.ShowDialog();
      }

      private void OnMainWindowClosing( object sender, CancelEventArgs e )
      {
         if ( _model.EncodingTasks.Any() )
         {
            e.Cancel = true;
         }
      }

      private void SelectFiles()
      {
         var dlg = new OpenFileDialog
         {
            Filter = "Video Files (*.mp4;*.wmv;*.webm)|*.mp4;*.wmv;*.webm|All files (*.*)|*.*",
            Multiselect = true
         };

         if ( dlg.ShowDialog() == true )
         {
            foreach( var file in dlg.FileNames )
            {
               if ( !_model.EncodingTasks.Any( x => x.Files.Contains( file ) ) )
               {
                  _model.SelectedFiles.Add( file );
               }
            }
         }
      }

      private void StartEncodingTask()
      {
         var encodingTask = new EncodingTask( _model.SelectedFiles.ToList(), _model.TargetFrameRate );

         lock ( _taskLock )
         {
            _model.EncodingTasks.Add( encodingTask.Model );
         }

         Task.Run( () => encodingTask.Start() ).ContinueWith( _ => FinishTask( encodingTask ) );

         _model.SelectedFiles.Clear();
      }

      private void FinishTask( EncodingTask encodingTask )
      {
         lock ( _taskLock )
         {
            Application.Current.Dispatcher.Invoke( () => _model.EncodingTasks.Remove( encodingTask.Model ) );
         }

         encodingTask?.Dispose();
      }
   }
}
