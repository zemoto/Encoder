namespace Encoder.Filters.Video.Denoise
{
   internal sealed class DenoiseVideoFilter : VideoFilter
   {
      public override VideoFilterViewModel ViewModel { get; } = new DenoiseVideoFilterViewModel();
      public override string FilterName { get; } = "Denoise";
   }
}
