using System.Windows;
using System.Windows.Controls;
using ZemotoUI;

namespace Encoder.UI
{
   internal sealed class GeneratedDataTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate( object item, DependencyObject container ) => DataTemplateFactory.ConstructDataTemplate( item as ViewModelBase );
   }
}
