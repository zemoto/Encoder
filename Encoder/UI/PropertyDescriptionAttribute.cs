using System;
using System.ComponentModel;

namespace Encoder.UI
{
   [AttributeUsage( AttributeTargets.Property )]
   internal class PropertyDescriptionAttribute : DescriptionAttribute
   {
      public double Min { get; }
      public double Max { get; }
      public string PropertyBeingDependedOn { get; }
      public object PropertyValueBeingDependedOn { get; }
      public bool HasDependency { get; }
      public bool HasMinMax { get; }

      public PropertyDescriptionAttribute( string label )
         : base( label )
      {
      }

      public PropertyDescriptionAttribute( string label, double min, double max )
         : this( label )
      {
         Min = min;
         Max = max;
         HasMinMax = true;
      }

      public PropertyDescriptionAttribute( string label, string propertyBeingDependedOn, object propertyValueBeingDependedOn )
         : this( label )
      {
         PropertyBeingDependedOn = propertyBeingDependedOn;
         PropertyValueBeingDependedOn = propertyValueBeingDependedOn;
         HasDependency = true;
      }

      public bool SharesDependencyWith( PropertyDescriptionAttribute other )
      {
         return string.Equals( PropertyBeingDependedOn, other.PropertyBeingDependedOn ) && 
                Equals( PropertyValueBeingDependedOn, other.PropertyValueBeingDependedOn );
      }
   }
}
