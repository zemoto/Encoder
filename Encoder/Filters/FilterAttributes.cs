using System;
using System.ComponentModel;
using Encoder.UI;

namespace Encoder.Filters
{
   [AttributeUsage( AttributeTargets.Class )]
   internal sealed class FilterAttribute : Attribute
   {
      public string FilterName { get; }

      public FilterAttribute( string filterName = "" )
      {
         FilterName = filterName;
      }
   }

   [AttributeUsage( AttributeTargets.Property )]
   internal sealed class FilterPropertyDescriptionAttribute : PropertyDescriptionAttribute
   {
      public string ArgumentParam { get; }
      
      public FilterPropertyDescriptionAttribute( string label, string argumentParam )
         : base( label )
      {
         ArgumentParam = argumentParam;
      }

      public FilterPropertyDescriptionAttribute( string label, string argumentParam, double min, double max )
         : base( label, min, max )
      {
         ArgumentParam = argumentParam;
      }

      public FilterPropertyDescriptionAttribute( string label, string argumentParam, string propertyBeingDependedOn, object propertyValueBeingDependedOn )
         : base( label, propertyBeingDependedOn, propertyValueBeingDependedOn )
      {
         ArgumentParam = argumentParam;
      }
   }

   [AttributeUsage( AttributeTargets.Field )]
   internal sealed class FilterEnumValueAttribute : DescriptionAttribute
   {
      public string ParameterValue { get; }

      public FilterEnumValueAttribute( string parameterValue )
      {
         ParameterValue = parameterValue;
      }

      public FilterEnumValueAttribute( string label, string parameterValue )
         : base( label )
      {
         ParameterValue = parameterValue;
      }
   }
}
