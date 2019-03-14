namespace Encoder.Filters
{
   internal abstract class Filter
   {
      public virtual FilterViewModel ViewModel { get; } = null;
      public abstract string FilterName { get; }
      public virtual string CustomFilterArguments { get; } = null;

      public virtual bool CanApplyFilter() => true;
   }
}
