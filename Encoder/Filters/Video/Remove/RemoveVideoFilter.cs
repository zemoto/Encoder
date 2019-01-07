namespace Encoder.Filters.Video.Remove
{
   internal sealed class RemoveVideoFilter : VideoFilter
   {
      public override FilterViewModel ViewModel { get; } = null;
      public override string FilterName { get; } = "Remove";
      public override string CustomFilterArguments { get; } = "-vn";
   }
}
