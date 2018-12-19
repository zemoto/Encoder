using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Encoder.UIUtils;
using Encoder.Utils;

namespace Encoder.Filters
{
   internal static class FilterDataTemplateFactory
   {
      public static DataTemplate ConstructDataTemplate( ViewModelBase filterViewModel )
      {
         var template = new DataTemplate { DataType = filterViewModel.GetType() };

         var mainGridFactory = CreateGridFactory( filterViewModel.GetType().GetProperties() );
         mainGridFactory.SetValue( FrameworkElement.MarginProperty, new Thickness( 0, 0, 0, 8 ) );

         template.VisualTree = mainGridFactory;
         return template;
      }

      private static FrameworkElementFactory CreateGridFactory( IReadOnlyList<PropertyInfo> properties )
      {
         var gridFactory = new FrameworkElementFactory( typeof( Grid ) );

         for ( int i = 0; i < properties.Count; i++ )
         {
            gridFactory.AppendChild( new FrameworkElementFactory( typeof( RowDefinition ) ) );

            var property = properties[i];
            var propertyControlFactory = new FrameworkElementFactory( typeof( FilterPropertyControl ) );
            var propertyAttribute = property.GetAttribute<FilterParameterAttribute>();
            propertyControlFactory.SetValue( FilterPropertyControl.LabelProperty, propertyAttribute.ParameterLabel + ":" );
            propertyControlFactory.SetValue( Grid.RowProperty, i );

            if ( property.PropertyType.IsEnum )
            {
               propertyControlFactory.AppendChild( ConstructComboBox( property ) );
            }
            else if ( property.PropertyType == typeof( bool ) )
            {
               // TODO
            }
            else
            {
               propertyControlFactory.AppendChild( ConstructTextBox( property, propertyAttribute ) );
            }
            gridFactory.AppendChild( propertyControlFactory );
         }

         return gridFactory;
      }

      private static FrameworkElementFactory ConstructComboBox( PropertyInfo property )
      {
         var comboBoxFactory = new FrameworkElementFactory( typeof( ComboBox ) );
         comboBoxFactory.SetValue( FrameworkElement.WidthProperty, 100.0 );
         comboBoxFactory.SetBinding( Selector.SelectedValueProperty, new Binding( property.Name ) );

         var enumValues = Enum.GetValues( property.PropertyType );
         var enumMembers = ( from object enumValue in enumValues select new BoundEnumMember( enumValue ) ).ToArray();
         comboBoxFactory.SetValue( Selector.SelectedValuePathProperty, "Value" );
         comboBoxFactory.SetValue( ItemsControl.DisplayMemberPathProperty, "Display" );
         comboBoxFactory.SetValue( ItemsControl.ItemsSourceProperty, enumMembers );

         return comboBoxFactory;
      }

      private static FrameworkElementFactory ConstructTextBox( PropertyInfo property, FilterParameterAttribute propertyAttribute )
      {
         var textBoxFactory = new FrameworkElementFactory( typeof( TextBox ) );

         if ( propertyAttribute.HasMinMax )
         {
            if ( property.PropertyType == typeof( int ) )
            {
               textBoxFactory.SetBinding( TextBox.TextProperty,
                  new IntMinMaxBinding( property.Name, (int)propertyAttribute.Min, (int)propertyAttribute.Max ) );
            }
            else
            {
               textBoxFactory.SetBinding( TextBox.TextProperty,
                  new DoubleMinMaxBinding( property.Name, propertyAttribute.Min, propertyAttribute.Max ) );
            }
         }
         else
         {
            textBoxFactory.SetResourceReference( TextBox.TextProperty, new Binding( property.Name ) );
         }

         return textBoxFactory;
      }
   }
}
