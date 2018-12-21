namespace Encoder.Filters.Video.Denoise
{
   internal sealed class DenoiseVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new DenoiseVideoFilterViewModel();
      public override string FilterName { get; } = "Denoise";
   }
}
