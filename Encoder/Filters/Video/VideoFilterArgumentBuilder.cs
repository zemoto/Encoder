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
            var filterParamName = property.GetAttribute<FilterParameterNameAttribute>().ParameterName;

            string filterParamValue;
            var propertyValue = property.GetValue( filter.ViewModel );
            if ( propertyValue is Enum )
            {
               var enumMember = propertyValue.GetType().GetMember( propertyValue.ToString() ).First();
               filterParamValue = enumMember.GetAttribute<FilterParameterValueAttribute>().ParameterValue;
            }
            else if ( propertyValue is bool boolValue )
            {
               filterParamValue = boolValue ? "1" : "0";
            }
            else
            {
               filterParamValue = propertyValue.ToString();
            }

            filterString += $"{filterParamName}={filterParamValue}:";
         }

         filterString = filterString.TrimEnd( ':' );
         filterString += "'\"";

         return filterString;
      }
   }
}
