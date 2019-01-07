using System;
using System.ComponentModel;

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
   internal sealed class FilterPropertyDescriptionAttribute : DescriptionAttribute
   {
      public string ArgumentParam { get; }
      
      public FilterPropertyDescriptionAttribute( string label, string argumentParam )
         : base( label )
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
