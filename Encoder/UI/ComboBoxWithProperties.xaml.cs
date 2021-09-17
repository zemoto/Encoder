using System.Windows;
using ZemotoUI;

namespace Encoder.UI
{
   internal partial class ComboBoxWithProperties
   {
      public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
         nameof( Label ),
         typeof( string ),
         typeof( ComboBoxWithProperties ),
         new PropertyMetadata( null ) );

      public string Label
      {
         get => (string)GetValue( LabelProperty );
         set => SetValue( LabelProperty, value );
      }

      public static readonly DependencyProperty OptionsContextProperty = DependencyProperty.Register(
         nameof( OptionsContext ),
         typeof( ViewModelBase ),
         typeof( ComboBoxWithProperties ),
         new PropertyMetadata( null ) );

      public ViewModelBase OptionsContext
      {
         get => (ViewModelBase)GetValue( OptionsContextProperty );
         set => SetValue( OptionsContextProperty, value );
      }

      public ComboBoxWithProperties()
      {
         InitializeComponent();
      }
   }
}
