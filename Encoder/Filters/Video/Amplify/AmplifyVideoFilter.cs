namespace Encoder.Filters.Video.Amplify
{
   internal sealed class AmplifyVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = new AmplifyVideoFilterViewModel();
      public override string FilterName { get; } = "Amplify";
   }
}
