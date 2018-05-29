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
            StartJobCommand = new RelayCommand( StartEncodingJob, () => _model.SelectedFiles?.Any() == true )
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
         if ( _model.EncodingJobs.Any() )
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
               if ( !_model.EncodingJobs.Any( x => x.Tasks.Any( y => y.SourceFile == file ) ) )
               {
                  _model.SelectedFiles.Add( file );
               }
            }
         }
      }

      private void StartEncodingJob()
      {
         var job = new EncodingJob( _model.SelectedFiles.ToList(), _model.TargetFrameRate );

         lock ( _taskLock )
         {
            _model.EncodingJobs.Add( job.Model );
         }

         Task.Run( () => job.Start() ).ContinueWith( _ => FinishJob( job ) );

         _model.SelectedFiles.Clear();
      }

      private void FinishJob( EncodingJob job )
      {
         lock ( _taskLock )
         {
            Application.Current.Dispatcher.Invoke( () => _model.EncodingJobs.Remove( job.Model ) );
         }

         job?.Dispose();
      }
   }
}
