using System;

namespace Encoder.Filters.Audio
{
   internal abstract class AudioFilter : Filter
   {
      public static AudioFilter GetFilterForType( AudioFilterType type )
      {
         var filterName = type.ToString();
         var filterType = Type.GetType( $"Encoder.Filters.Audio.{filterName}.{filterName}AudioFilter" );
         if ( filterType != null )
         {
            return (AudioFilter)Activator.CreateInstance( filterType );
         }

         throw new NotImplementedException( "Filter not implemented, named incorrectly, or in wrong namespace" );
      }
   }
}
