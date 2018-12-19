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
      public string PropertyBeingDependedOn { get; }
      public object PropertyValueBeingDependedOn { get; }
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

      public FilterParameterAttribute( string label, string argumentParam, string propertyBeingDependedOn, object propertyValueBeingDependedOn )
         : this( label, argumentParam )
      {
         PropertyBeingDependedOn = propertyBeingDependedOn;
         PropertyValueBeingDependedOn = propertyValueBeingDependedOn;
         HasDependency = true;
      }

      public bool SharesDependencyWith( FilterParameterAttribute other )
      {
         return string.Equals( PropertyBeingDependedOn, other.PropertyBeingDependedOn ) && 
                       Equals( PropertyValueBeingDependedOn, other.PropertyValueBeingDependedOn );
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
