using Nez;

namespace Raven
{
    public class SelectionList 
  {
    public List<object> Selections = new List<object>();

    // public Enumerable
    public void Add(object sel)
    {
      if (!Nez.InputUtils.IsShiftDown())
        Selections.Clear();

      Selections.AddIfNotPresent(sel);
    }
    public object Last() => Selections.Last();
    public bool NotEmpty() => Selections.Count() > 0;
}
}
