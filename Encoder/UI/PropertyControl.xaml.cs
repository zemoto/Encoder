using System.Windows;

namespace Encoder.UI
{
   internal partial class PropertyControl
   {
      public static readonly DependencyProperty LabelProperty =
         DependencyProperty.Register( nameof( Label ), typeof( string ),
                                      typeof( PropertyControl ), new PropertyMetadata( string.Empty ) );
      public string Label
      {
         get => (string)GetValue( LabelProperty );
         set => SetValue( LabelProperty, value );
      }

      public PropertyControl()
      {
         InitializeComponent();
      }
   }
}
