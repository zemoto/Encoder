namespace Encoder.Filters.Video.Blur
{
   internal sealed class BlurFilter : Filter
   {
      public override FilterViewModel ViewModel { get; } = new BlurFilterViewModel();
      public override string FilterName { get; } = "Blur";
   }
}
