using System.Windows;

namespace Encoder.Filters
{
   internal partial class FilterPropertyControl
   {
      public static readonly DependencyProperty LabelProperty = 
         DependencyProperty.Register( nameof( Label ), typeof( string ),
                                      typeof( FilterPropertyControl ), new PropertyMetadata( string.Empty ) );
      public string Label
      {
         get => (string)GetValue( LabelProperty );
         set => SetValue( LabelProperty, value );
      }

      public FilterPropertyControl()
      {
         InitializeComponent();
      }
   }
}
