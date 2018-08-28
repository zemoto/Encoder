namespace Encoder.Filters.Video.Denoise
{
   internal sealed class DenoiseFilter : Filter
   {
      public override FilterViewModel ViewModel { get; } = new DenoiseFilterViewModel();
      public override string FilterName { get; } = "Denoise";
   }
}
