using System;
using System.Linq;
using Encoder.Filters.Video.Copy;
using Encoder.Utils;

namespace Encoder.Filters.Video
{
   internal static class VideoFilterArgumentBuilder
   {
      public static string GetFilterArguments( VideoFilter filter )
      {
         if ( !filter.CanApplyFilter() )
         {
            return new CopyVideoFilter().CustomFilterArguments;
         }
         if ( !string.IsNullOrEmpty( filter.CustomFilterArguments ) )
         {
            return filter.CustomFilterArguments;
         }

         var filterString = "-filter:v ";

         var filterName = filter.ViewModel.GetType().GetAttribute<FilterAttribute>().FilterName;
         filterString += $"\"{filterName}='";

         var filterProperties = filter.ViewModel.GetType().GetProperties();
         foreach( var property in filterProperties )
         {
            var filterParamAttribute = property.GetAttribute<FilterParameterAttribute>();

            if ( filterParamAttribute.HasDependency )
            {
               var propertyBeingDependedOn = filterProperties.FirstOrDefault( x => string.Equals( x.Name, filterParamAttribute.PropertyDependency ) );
               if ( !Equals( propertyBeingDependedOn?.GetValue( filter.ViewModel ), filterParamAttribute.DependencyValue) )
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
