using System.Windows;
using ZemotoCommon.UI;

namespace Encoder.Filters
{
   internal partial class FilterSelectorControl
   {
      public static readonly DependencyProperty LabelProperty = DependencyProperty.Register( 
         nameof( Label ),
         typeof( string ),
         typeof( FilterSelectorControl ),
         new PropertyMetadata( string.Empty ) );

      public string Label
      {
         get => (string)GetValue( LabelProperty );
         set => SetValue( LabelProperty, value );
      }

      public static readonly DependencyProperty OptionsContextProperty = DependencyProperty.Register( 
         nameof( OptionsContext ),
         typeof( ViewModelBase ),
         typeof( FilterSelectorControl ),
         new PropertyMetadata( null ) );

      public ViewModelBase OptionsContext
      {
         get => (ViewModelBase)GetValue( OptionsContextProperty );
         set => SetValue( OptionsContextProperty, value );
      }

      public FilterSelectorControl()
      {
         InitializeComponent();
      }
   }
}
