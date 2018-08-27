namespace Encoder.Filters.Video.Amplify
{
   internal sealed class AmplifyFilter : Filter
   {
      public override FilterViewModel ViewModel { get; } = new AmplifyFilterViewModel();
      public override string FilterName { get; } = "Amplify";
   }
}
