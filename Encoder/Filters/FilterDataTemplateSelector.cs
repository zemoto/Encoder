using System.Windows;
using System.Windows.Controls;
using Encoder.Utils;

namespace Encoder.Filters
{
   internal sealed class FilterDataTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate( object item, DependencyObject container )
      {
         var filterAttribute = item?.GetType().GetAttribute<FilterAttribute>();
         if ( filterAttribute == null )
         {
            return null;
         }

         if ( filterAttribute.ControlType != null )
         {
            return new DataTemplate { VisualTree = new FrameworkElementFactory( filterAttribute.ControlType ) };
         }

         return FilterDataTemplateFactory.ConstructDataTemplate( item as ViewModelBase );
      }
   }
}
