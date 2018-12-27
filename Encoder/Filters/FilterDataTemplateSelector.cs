using System.Windows;
using System.Windows.Controls;
using ZemotoCommon.UI;

namespace Encoder.Filters
{
   internal sealed class FilterDataTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate( object item, DependencyObject container ) => FilterDataTemplateFactory.ConstructDataTemplate( item as ViewModelBase );
   }
}
