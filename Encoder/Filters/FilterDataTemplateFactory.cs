﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Encoder.UIUtils;
using Encoder.Utils;

namespace Encoder.Filters
{
   internal static class FilterDataTemplateFactory
   {
      public static DataTemplate ConstructDataTemplate( ViewModelBase filterViewModel )
      {
         if ( filterViewModel == null )
         {
            return null;
         }

         var template = new DataTemplate { DataType = filterViewModel.GetType() };

         var properties = filterViewModel.GetType().GetProperties();
         if ( properties.Length > 0 )
         {
            var mainGridFactory = CreateGridFactory( properties, false );
            mainGridFactory.SetValue( FrameworkElement.MarginProperty, new Thickness( 0, 0, 0, 8 ) );
            template.VisualTree = mainGridFactory;
         }
         
         return template;
      }

      private static FrameworkElementFactory CreateGridFactory( IReadOnlyList<PropertyInfo> properties, bool ignoreDependency )
      {
         var gridFactory = new FrameworkElementFactory( typeof( Grid ) );

         for ( int i = 0; i < properties.Count; i++ )
         {
            int row = i;
            var rowDef = new FrameworkElementFactory( typeof( RowDefinition ) );
            rowDef.SetValue( RowDefinition.HeightProperty, new GridLength( 0, GridUnitType.Auto ) );
            gridFactory.AppendChild( rowDef );

            var property = properties[i];
            var propertyAttribute = property.GetAttribute<FilterParameterAttribute>();

            FrameworkElementFactory elementFactory;
            if ( propertyAttribute.HasDependency && !ignoreDependency )
            {
               // Take every adjacent property dependent on the same property and next them in a grid
               var dependentProperties = new List<PropertyInfo>();
               int j = i;
               for ( ; j < properties.Count; j++ )
               {
                  var otherPropertyAttribute = properties[j].GetAttribute<FilterParameterAttribute>();
                  if ( propertyAttribute.SharesDependencyWith( otherPropertyAttribute ) )
                  {
                     dependentProperties.Add( properties[j] );
                  }
                  else
                  {
                     break;
                  }
               }
               i = j - 1; // Skip the properties we are about to nest in another grid

               elementFactory = CreateGridFactory( dependentProperties, true );
               elementFactory.SetValue( FrameworkElement.MarginProperty, new Thickness( 16, 0, 0, 0 ) );

               var visibilityBinding = new Binding( propertyAttribute.PropertyBeingDependedOn )
               {
                  Converter = new EqualityToVisibilityConverter(),
                  ConverterParameter = propertyAttribute.PropertyValueBeingDependedOn
               };
               elementFactory.SetBinding( UIElement.VisibilityProperty, visibilityBinding );

               // Add a thin rectangle on the left side of the nested grid for styling
               var indentIndicatorFactory = CreateIndentIndicatorFactory();
               indentIndicatorFactory.SetValue( Grid.RowProperty, row );
               indentIndicatorFactory.SetBinding( UIElement.VisibilityProperty, visibilityBinding );
               gridFactory.AppendChild( indentIndicatorFactory );
            }
            else
            {
               elementFactory = ConstructFilterPropertyControl( property, propertyAttribute );
            }
            elementFactory.SetValue( Grid.RowProperty, row );
            
            gridFactory.AppendChild( elementFactory );
         }

         return gridFactory;
      }

      private static FrameworkElementFactory ConstructFilterPropertyControl( PropertyInfo property, FilterParameterAttribute propertyAttribute )
      {
         var propertyControlFactory = new FrameworkElementFactory( typeof( FilterPropertyControl ) );
         propertyControlFactory.SetValue( FilterPropertyControl.LabelProperty, propertyAttribute.ParameterLabel + ":" );

         if ( property.PropertyType.IsEnum )
         {
            propertyControlFactory.AppendChild( ConstructComboBox( property ) );
         }
         else if ( property.PropertyType == typeof( bool ) )
         {
            propertyControlFactory.AppendChild( ConstructCheckBox( property ) );
         }
         else
         {
            propertyControlFactory.AppendChild( ConstructTextBox( property, propertyAttribute ) );
         }
         return propertyControlFactory;
      }

      private static FrameworkElementFactory CreateIndentIndicatorFactory()
      {
         var indentIndicatorFactory = new FrameworkElementFactory( typeof( Rectangle ) );
         indentIndicatorFactory.SetValue( FrameworkElement.WidthProperty, 1.0 );
         indentIndicatorFactory.SetValue( FrameworkElement.MarginProperty, new Thickness( 8, 0, 0, 0 ) );
         indentIndicatorFactory.SetValue( Shape.StrokeThicknessProperty, 1.0 );
         indentIndicatorFactory.SetValue( Shape.StrokeProperty, Brushes.DarkGray );
         indentIndicatorFactory.SetValue( FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left );

         return indentIndicatorFactory;
      }

      private static FrameworkElementFactory ConstructComboBox( PropertyInfo property )
      {
         var comboBoxFactory = new FrameworkElementFactory( typeof( ComboBox ) );
         comboBoxFactory.SetBinding( Selector.SelectedValueProperty, new Binding( property.Name ) );

         var enumValues = Enum.GetValues( property.PropertyType );
         var enumMembers = ( from object enumValue in enumValues select new BoundEnumMember( enumValue ) ).ToArray();
         comboBoxFactory.SetValue( Selector.SelectedValuePathProperty, "Value" );
         comboBoxFactory.SetValue( ItemsControl.DisplayMemberPathProperty, "Display" );
         comboBoxFactory.SetValue( ItemsControl.ItemsSourceProperty, enumMembers );

         comboBoxFactory.SetValue( FrameworkElement.MarginProperty, new Thickness( 0, 4, 0, 4 ) );
         comboBoxFactory.SetValue( Control.VerticalContentAlignmentProperty, VerticalAlignment.Center );
         comboBoxFactory.SetValue( FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center );
         comboBoxFactory.SetValue( FrameworkElement.HeightProperty, 24.0 );
         comboBoxFactory.SetValue( Control.PaddingProperty, new Thickness( 8, 0, 0, 0 ) );

         return comboBoxFactory;
      }

      private static FrameworkElementFactory ConstructCheckBox( PropertyInfo property )
      {
         var checkBoxFactory = new FrameworkElementFactory( typeof( CheckBox ) );

         checkBoxFactory.SetBinding( ToggleButton.IsCheckedProperty, new Binding( property.Name ) );
         checkBoxFactory.SetValue( FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center );

         return checkBoxFactory;
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
            textBoxFactory.SetBinding( TextBox.TextProperty, new Binding( property.Name ) );
         }

         textBoxFactory.SetValue( FrameworkElement.MarginProperty, new Thickness( 0, 4, 0, 4 ) );
         textBoxFactory.SetValue( Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center );
         textBoxFactory.SetValue( Control.VerticalContentAlignmentProperty, VerticalAlignment.Center );
         textBoxFactory.SetValue( FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left );
         textBoxFactory.SetValue( FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center );
         textBoxFactory.SetValue( FrameworkElement.WidthProperty, 45.0 );
         textBoxFactory.SetValue( FrameworkElement.HeightProperty, 24.0 );

         return textBoxFactory;
      }
   }
}