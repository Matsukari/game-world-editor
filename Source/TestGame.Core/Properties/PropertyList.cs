  

namespace Raven 
{
  /// <summary>
  /// A list of objects of arbitrary type. 
  /// </summary>
  public class PropertyList : ICloneable
  {
    public Dictionary<string, object> Data = new Dictionary<string, object>();

    public void Add<T>(T obj, string name="")
    {
      if (name == string.Empty) name = obj.GetType().Name;
      Data.AddWithUniqueName(name, obj);
    }
    public PropertyList Copy() 
    {
      var copy = new Dictionary<string, object>();
      foreach (var prop  in Data)
      {
        if (prop.Value.GetType().IsByRef && !(prop.Value is ICloneable))
        {
          throw new Exception("Err copy with a non clonable property");
        }
        copy.AddWithUniqueName(prop.Key, prop.Value);
      }
      var list = new PropertyList();
      list.Data = copy;
      return list;
    }
    object ICloneable.Clone() => Copy();

    public void OverrideOrAddAll(PropertyList properties) 
    {
      foreach (var prop in properties)
      {
        Data[prop.Key] = prop.Value;
      }
    }
    public void Remove(string name) => Data.Remove(name);
    public Dictionary<string, object>.Enumerator GetEnumerator() { return Data.GetEnumerator(); }
  }
}
