using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Encoder.TaskCreation;
using Encoder.Encoding;
using Encoder.Encoding.Tasks;
using ZemotoUI;

namespace Encoder
{
   internal sealed class Main : IDisposable
   {
      private readonly MainWindowViewModel _model;
      private readonly EncodingManager _encodingManager;

      public Main()
      {
         _encodingManager = new EncodingManager();

         var creationViewModel = new TaskCreationViewModel( _encodingManager );

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
               await Task.Delay( 300 ).ConfigureAwait( false );
            }

            ( (Window)sender ).Close();
         }
      }
   }
}
