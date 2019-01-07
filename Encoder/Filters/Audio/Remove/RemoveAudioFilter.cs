namespace Encoder.Filters.Audio.Remove
{
   internal sealed class RemoveAudioFilter : AudioFilter
   {
      public override FilterViewModel ViewModel { get; } = null;
      public override string FilterName { get; } = "Remove";
      public override string CustomFilterArguments { get; } = "-an";
   }
}
