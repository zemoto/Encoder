namespace Encoder.Filters.Video.Blur
{
   internal sealed class BlurVideoFilter : VideoFilter
   {
      public override VideoFilterViewModel ViewModel { get; } = new BlurVideoFilterViewModel();
      public override string FilterName { get; } = "Blur";
   }
}
