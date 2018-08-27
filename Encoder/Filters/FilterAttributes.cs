using System;

namespace Encoder.Filters
{
   [AttributeUsage( AttributeTargets.Class )]
   internal sealed class FilterAttribute : Attribute
   {
      public string FilterName { get; }
      public Type ControlType { get; }

      public FilterAttribute( string filterName, Type controlType )
      {
         FilterName = filterName;
         ControlType = controlType;
      }
   }

   [AttributeUsage( AttributeTargets.Property )]
   internal sealed class FilterParameterNameAttribute : Attribute
   {
      public string ParameterName { get; }

      public FilterParameterNameAttribute( string parameterName )
      {
         ParameterName = parameterName;
      }
   }

   [AttributeUsage( AttributeTargets.Field )]
   internal sealed class FilterParameterValueAttribute : Attribute
   {
      public string ParameterValue { get; }

      public FilterParameterValueAttribute( string parameterValue )
      {
         ParameterValue = parameterValue;
      }
   }
}
