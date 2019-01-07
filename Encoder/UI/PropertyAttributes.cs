using System;

namespace Encoder.UI
{
   [AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
   internal sealed class PropertyDependencyAttribute : Attribute, IEquatable<PropertyDependencyAttribute>
   {
      public string PropertyBeingDependedOn { get; }
      public object PropertyValueBeingDependedOn { get; }

      public PropertyDependencyAttribute( string propertyBeingDependedOn, object propertyValueBeingDependedOn )
      {
         PropertyBeingDependedOn = propertyBeingDependedOn;
         PropertyValueBeingDependedOn = propertyValueBeingDependedOn;
      }

      public bool Equals( PropertyDependencyAttribute other )
      {
         return other != null && 
                string.Equals( PropertyBeingDependedOn, other.PropertyBeingDependedOn ) &&
                Equals( PropertyValueBeingDependedOn, other.PropertyValueBeingDependedOn );
      }
   }

   [AttributeUsage( AttributeTargets.Property )]
   internal sealed class PropertyMinMaxAttribute : Attribute
   {
      public double Min { get; }
      public double Max { get; }

      public PropertyMinMaxAttribute( double min, double max )
      {
         Min = min;
         Max = max;
      }
   }
}
