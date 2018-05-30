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

      public Main()
      {
         _model = new MainWindowViewModel
         {
            SelectFilesCommand = new RelayCommand( SelectFiles ),
            RemoveFileCommand = new RelayCommand<string>( file => _model.SelectedFiles.Remove( file ) ),
            StartJobCommand = new RelayCommand( StartEncodingJob, () => _model.SelectedFiles.Any() == true )
         };
      }

      public void ShowDialog()
      {
         var window = new MainWindow( _model );
         window.Closing += OnMainWindowClosing;
         window.ShowDialog();
      }

      private async void OnMainWindowClosing( object sender, CancelEventArgs e )
      {
         if ( !_model.EncodingJobs.Any() )
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
            foreach ( var job in _model.EncodingJobs )
            {
               job.StopJobCommand.Execute( null );
            }
            while ( _model.EncodingJobs.Any() )
            {
               await Task.Delay( 300 );
            }
            ( sender as Window ).Close();
         }
      }

      private void SelectFiles()
      {
         var dlg = new OpenFileDialog
         {
            Filter = "Video Files (*.mp4;*.wmv;*.webm;*.swf;*.mkv)|*.mp4;*.wmv;*.webm;*.swf;*.mkv|All files (*.*)|*.*",
            Multiselect = true
         };

         if ( dlg.ShowDialog( Application.Current.MainWindow ) == true )
         {
            foreach( var file in dlg.FileNames )
            {
               if ( !_model.SelectedFiles.Contains( file ) && 
                    !_model.EncodingJobs.Any( x => x.Tasks.Any( y => y.SourceFile == file ) ) )
               {
                  _model.SelectedFiles.Add( file );
               }
            }
         }
      }

      private void StartEncodingJob()
      {
         Task.Run( () =>
         {
            var job = new EncodingJob( _model.SelectedFiles.ToList(), _model.TargetFrameRate );
            Application.Current.Dispatcher.Invoke( () =>
            {
               _model.SelectedFiles.Clear();
               _model.EncodingJobs.Add( job.Model );
            } );

            job.DoJob();

            Application.Current.Dispatcher.Invoke( () => _model.EncodingJobs.Remove( job.Model ) );

            job.Dispose();
         } );

      }
   }
}
