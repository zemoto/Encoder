using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Interpolator.Encoding;
using Interpolator.JobCreation;
using Interpolator.Utils;

namespace Interpolator
{
   internal sealed class Main
   {
      private readonly MainWindowViewModel _model;

      public Main()
      {
         _model = new MainWindowViewModel
         {
            NewJobCommand = new RelayCommand( async () => await CreateAndStartNewJobAsync() )
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

      private async Task CreateAndStartNewJobAsync()
      {
         var jobWizard = new JobCreationWizard();
         var job = jobWizard.CreateJob();

         if ( job != null )
         {
            await StartJobAsync( job );
            job.Dispose();
         }
      }

      private async Task StartJobAsync( EncodingJob job )
      {
         _model.EncodingJobs.Add( job.Model );

         await job.DoJobAsync();

         _model.EncodingJobs.Remove( job.Model );

      }
   }
}
