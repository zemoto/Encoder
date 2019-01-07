using System;
using System.Linq;
using Encoder.Filters.Audio;
using Encoder.Filters.Video;
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
            var filterParamAttribute = property.GetAttribute<FilterPropertyDescriptionAttribute>();

            if ( filterParamAttribute.HasDependency )
            {
               var propertyBeingDependedOn = filterProperties.FirstOrDefault( x => string.Equals( x.Name, filterParamAttribute.PropertyBeingDependedOn ) );
               if ( !Equals( propertyBeingDependedOn?.GetValue( filter.ViewModel ), filterParamAttribute.PropertyValueBeingDependedOn) )
               {
                  continue;
               }
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

            filterString += $"{filterParamAttribute.ArgumentParam}={filterParamValue}:";
         }

         filterString = filterString.TrimEnd( ':' );
         filterString += "'\"";

         return filterString;
      }
   }
}
