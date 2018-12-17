namespace Encoder.Filters.Video.Encode
{
   internal sealed class EncodeVideoFilter : VideoFilter
   {
      public override VideoFilterViewModel ViewModel { get; }
      public override string FilterName { get; } = "Encode";
      public override string CustomFilterArguments { get; } = "-c:v libx264";
   }
}
