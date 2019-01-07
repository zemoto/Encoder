using System;
using System.Linq;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;
using Encoder.UI;
using ZemotoCommon.Utils;

namespace Encoder.Filters
{
   internal static class FilterArgumentBuilder
   {
      public static string GetFilterArguments( VideoFilter filter ) => GetFilterArguments( filter, "v" );

      public static string GetFilterArguments( AudioFilter filter ) => GetFilterArguments( filter, "a" );

      private static string GetFilterArguments( Filter filter, string filterType )
      {
         if ( !filter.CanApplyFilter() )
         {
            return $"-c:{filterType} copy";
         }
         if ( !string.IsNullOrEmpty( filter.CustomFilterArguments ) )
         {
            return filter.CustomFilterArguments;
         }

         var filterString = $"-filter:{filterType} ";

         var filterName = filter.ViewModel.GetType().GetAttribute<FilterAttribute>().FilterName;
         filterString += $"\"{filterName}='";

         var filterProperties = filter.ViewModel.GetType().GetProperties();
         foreach( var property in filterProperties )
         {
            var dependencyAttributes = property.GetAttributes<PropertyDependencyAttribute>();

            bool dependenciesMet = true;
            foreach ( var dependency in dependencyAttributes )
            {
               var propertyBeingDependedOn = filterProperties.FirstOrDefault( x => string.Equals( x.Name, dependency.PropertyBeingDependedOn ) );
               if ( !Equals( propertyBeingDependedOn?.GetValue( filter.ViewModel ), dependency.PropertyValueBeingDependedOn ) )
               {
                  dependenciesMet = false;
                  break;
               }
            }

            if ( !dependenciesMet )
            {
               continue;
            }

            string filterParamValue;
            var propertyValue = property.GetValue( filter.ViewModel );
            if ( propertyValue is Enum enumValue )
            {
               filterParamValue = enumValue.GetAttribute<FilterEnumValueAttribute>().ParameterValue;
            }
            else if ( propertyValue is bool boolValue )
            {
               filterParamValue = boolValue ? "1" : "0";
            }
            else
            {
               filterParamValue = propertyValue.ToString();
            }

            var filterParamAttribute = property.GetAttribute<FilterPropertyDescriptionAttribute>();
            filterString += $"{filterParamAttribute.ArgumentParam}={filterParamValue}:";
         }

         filterString = filterString.TrimEnd( ':' );
         filterString += "'\"";

         return filterString;
      }
   }
}
