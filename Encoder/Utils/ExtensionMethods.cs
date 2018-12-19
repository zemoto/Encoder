﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Encoder.Filters.Video;

namespace Encoder.Utils
{
   internal static class ExtensionMethods
   {
      public static void StartAsChildProcess( this Process process )
      {
         process.Start();
         ChildProcessWatcher.AddProcess( process );
      }

      public static void ForEach<T>( this IEnumerable<T> collection, Action<T> action )
      {
         collection.ToList().ForEach( action );
      }

      public static T GetAttribute<T>( this Enum enumValue ) where T : Attribute
      {
         var enumValueInfo = enumValue.GetType().GetMember( enumValue.ToString() ).First();
         return GetAttribute<T>( enumValueInfo );
      }

      public static T GetAttribute<T>( this ICustomAttributeProvider property ) where T : Attribute
      {
         var attribute = property.GetCustomAttributes( typeof( T ), false ).FirstOrDefault();
         return attribute as T;
      }

      public static VideoFilter GetFilterForType( this VideoFilterType type )
      {
         var filterName = type.ToString();
         var filterType = Type.GetType( $"Encoder.Filters.Video.{filterName}.{filterName}VideoFilter" );
         if ( filterType != null )
         {
            return (VideoFilter)Activator.CreateInstance( filterType );
         }

         throw new NotImplementedException( "Filter not implemented, named incorrectly, or in wrong namespace" );
      }
   }
}
