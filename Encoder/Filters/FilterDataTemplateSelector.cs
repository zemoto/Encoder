using System.Windows;
using System.Windows.Controls;
using Encoder.Utils;

namespace Encoder.Filters
{
   internal sealed class FilterDataTemplateSelector : DataTemplateSelector
   {
      public override DataTemplate SelectTemplate( object item, DependencyObject container )
      {
         if ( item == null )
         {
            return null;
         }

         var template = new DataTemplate();

         var filterAttribute = item.GetType().GetAttribute<FilterAttribute>();
         if ( filterAttribute != null )
         {
            template.VisualTree = new FrameworkElementFactory( filterAttribute.ControlType );
         }

         return template;
      }
   }
}
