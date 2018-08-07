using System;

namespace Interpolator.Filters
{
   [AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
   internal sealed class FilterAttribute : Attribute
   {
      public string FilterName { get; set; }

      public FilterAttribute( string filterName )
      {
         FilterName = filterName;
      }
   }

   [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
   internal sealed class FilterParameterNameAttribute : Attribute
   {
      public string ParameterName { get; set; }

      public FilterParameterNameAttribute( string parameterName )
      {
         ParameterName = parameterName;
      }
   }

   [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
   internal sealed class FilterParameterValueAttribute : Attribute
   {
      public string ParameterValue { get; set; }

      public FilterParameterValueAttribute( string parameterValue )
      {
         ParameterValue = parameterValue;
      }
   }
}
