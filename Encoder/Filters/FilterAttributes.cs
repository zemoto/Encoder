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
   internal sealed class FilterParameterAttribute : Attribute
   {
      public string ParameterLabel { get; }
      public string ArgumentParam { get; }
      public string PropertyDependency { get; }
      public object DependencyValue { get; }
      public bool HasDependency { get; }
      public double Min { get; }
      public double Max { get; }
      public bool HasMinMax { get; }

      public FilterParameterAttribute( string argumentParam )
      {
         ArgumentParam = argumentParam;
      }

      public FilterParameterAttribute( string label, string argumentParam )
         : this( argumentParam )
      {
         ParameterLabel = label;
      }

      public FilterParameterAttribute( string label, string argumentParam, double min, double max )
         : this( label, argumentParam )
      {
         Min = min;
         Max = max;
         HasMinMax = true;
      }

      public FilterParameterAttribute( string label, string argumentParam, string propertyDependency, object dependencyValue )
         : this( label, argumentParam )
      {
         PropertyDependency = propertyDependency;
         DependencyValue = dependencyValue;
         HasDependency = true;
      }

      public bool SharesDependencyWith( FilterParameterAttribute other )
      {
         return string.Equals( PropertyDependency, other.PropertyDependency ) && Equals( DependencyValue, other.DependencyValue );
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
