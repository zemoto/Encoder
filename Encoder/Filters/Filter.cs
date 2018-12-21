namespace Encoder.Filters
{
   internal abstract class Filter
   {
      public abstract FilterViewModel ViewModel { get; }
      public abstract string FilterName { get; }
      public virtual string CustomFilterArguments { get; } = null;

      public virtual bool CanApplyFilter() => true;
   }
}
