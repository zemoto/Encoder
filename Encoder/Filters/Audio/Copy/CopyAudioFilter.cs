namespace Encoder.Filters.Audio.Copy
{
   internal sealed class CopyAudioFilter : AudioFilter
   {
      public override FilterViewModel ViewModel { get; }
      public override string FilterName { get; } = "Copy";
      public override string CustomFilterArguments { get; } = "-c:a copy";
   }
}
