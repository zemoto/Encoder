using System;

namespace Encoder.Filters
{
   [AttributeUsage( AttributeTargets.Class )]
   internal sealed class FilterAttribute : Attribute
   {
      public string FilterName { get; }
      public Type ControlType { get; }

      public FilterAttribute( string filterName = "", Type controlType = null )
      {
         FilterName = filterName;
         ControlType = controlType;
      }
   }

   [AttributeUsage( AttributeTargets.Property )]
   internal sealed class FilterParameterAttribute : Attribute
   {
      public string ParameterLabel { get; }
      public string ArgumentValue { get; }
      public double Min { get; }
      public double Max { get; }
      public bool HasMinMax => Math.Abs( Min - Max ) > 0.001;

      public FilterParameterAttribute( string argumentValue )
      {
         ArgumentValue = argumentValue;
      }

      public FilterParameterAttribute( string label, string argumentValue, double min = 0, double max = 0 )
      {
         ParameterLabel = label;
         ArgumentValue = argumentValue;
         Min = min;
         Max = max;
      }
   }

   [AttributeUsage( AttributeTargets.Field )]
   internal sealed class FilterEnumValueAttribute : Attribute
   {
      public string ParameterValue { get; }

      public FilterEnumValueAttribute( string parameterValue )
      {
         ParameterValue = parameterValue;
      }
   }
}
