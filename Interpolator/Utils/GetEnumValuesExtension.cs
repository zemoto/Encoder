using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Interpolator.Utils
{
   internal sealed class BoundEnumMember
   {
      public BoundEnumMember( object value )
      {
         Display = GetDescription( value );
         Value = value;
      }

      public string Display { get; set; }
      public object Value { get; set; }

      private string GetDescription( object value )
      {
         var enumMember = value.GetType().GetMember( value.ToString() ).FirstOrDefault();
         if ( enumMember != null )
         {
            var description = enumMember.GetAttribute<DescriptionAttribute>()?.Description;
            if ( !string.IsNullOrEmpty( description ) )
            {
               return description;
            }
         }
         return value.ToString();
      }
   }

   internal sealed class GetEnumValuesExtension : MarkupExtension
   {
      private readonly Type _type;
      public GetEnumValuesExtension( Type type )
      {
         _type = type;
      }

      public override object ProvideValue( IServiceProvider serviceProvider )
      {
         var target = serviceProvider.GetService( typeof(IProvideValueTarget) ) as IProvideValueTarget;
         if ( target != null && target.TargetObject is ComboBox )
         {
            ( (ComboBox)target.TargetObject ).SelectedValuePath = "Value";
            ( (ComboBox)target.TargetObject ).DisplayMemberPath = "Display";
         }

         var enumValues = Enum.GetValues( _type );

         return ( from object enumValue in enumValues
                  select new BoundEnumMember( enumValue ) ).ToArray();
      }
   }
}
