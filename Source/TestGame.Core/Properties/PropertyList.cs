  

namespace Raven 
{
  /// <summary>
  /// A list of objects of arbitrary type. 
  /// </summary>
  public class PropertyList : ICloneable
  {
    public Dictionary<string, object> Data = new Dictionary<string, object>();

    int _counter = 0;
    public void Add<T>(T obj)
    {
      string name = $"{obj.GetType().Name}.{_counter++}";
      if (obj is IPropertied prop) 
      {
        if (prop.Name != "") 
        {
          name = prop.Name;
        }
      }
      Data.TryAdd(name, obj);
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
        copy.TryAdd(prop.Key, prop.Value);
      }
      var list = new PropertyList();
      list.Data = copy;
      list._counter = _counter;
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
