using Encoder.Encoding.Tasks;

namespace Encoder.Operations
{
   internal sealed class CustomParamsOperation : Operation
   {
      public override string Name => $"Custom Params - {_customParams}";

      private readonly string _customParams;
      private readonly string _extension;

      public CustomParamsOperation( string customParams, string extension )
      {
         _customParams = customParams;
         _extension = extension;
      }

      public override EncodingTask CreateEncodingTask() => new EncodeWithCustomParams( new EncodingParams( "Custom", _customParams, _extension ) );
   }
}
