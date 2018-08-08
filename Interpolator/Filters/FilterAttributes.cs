using System;

namespace Interpolator.Filters
{
   [AttributeUsage( AttributeTargets.Class )]
   internal sealed class FilterAttribute : Attribute
   {
      public string FilterName { get; }

      public FilterAttribute( string filterName )
      {
         FilterName = filterName;
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
