﻿using Encoder.Utils;

namespace Encoder.Filters.Video.Encode
{
   internal sealed class EncodeVideoFilter : VideoFilter
   {
      public override VideoFilterViewModel ViewModel { get; } = new EncodeVideoFilterViewModel();
      public override string FilterName { get; } = "Encode";

      public override string CustomFilterArguments
      {
         get
         {
            var fileFormat = ( (EncodeVideoFilterViewModel)ViewModel ).FileFormat;
            return $"-c:v {fileFormat.GetAttribute<FilterParameterValueAttribute>().ParameterValue}";
         }
      }
   }
}