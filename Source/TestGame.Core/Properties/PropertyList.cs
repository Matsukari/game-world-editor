
namespace Raven 
{
  /// <summary>
  /// A list of objects of custom type. Note that this will only properly work if the added property has a renderer on the editor, basic properties include: string, int, float, boolean, Color, ShapeModels (rectangle, ellipse, polygon, point)
  /// </summary>
  public class PropertyList : ICloneable
  {
    public class Property 
    {
      public string Key;
      public Object Value;

      private Property() 
      {
      }
      public Property(string name, object data) 
      {
        Key = name;
        Value = data;
      }
    }
    public List<Property> Data = new List<Property>();

    public T Get<T>(string name) => (T)(Data.Find(item => item.Key == name).Value);
    public void Set(string name, object obj) => Data.Find(item => item.Key == name).Value = obj;

    public bool Contains(string name) => Data.Find(item => item.Key == name) != null;

    public void Add<T>(T obj, string name="")
    {
      if (name == string.Empty) name = obj.GetType().Name;
      Data.Add(new Property(name, obj));
    }

    public void AddOrSet<T>(T obj, string name)
    {
      if (Contains(name)) Set(name, obj);
      else Add(obj, name);
    }

    public PropertyList Copy() 
    {
      var list = new PropertyList();
      foreach (var prop in Data)
      {
        var n = prop.Value;
        if (prop.Value.GetType().IsByRef && !(prop.Value is ICloneable))
        {
          throw new Exception("Err copy with a non clonable property");
        }
        else if (prop.Value.GetType().IsByRef)
        {
          n = (prop as ICloneable).Clone();
        }
        list.Add(new Property(prop.Key, n));
      }
      return list;
    }
    object ICloneable.Clone() => Copy();

    public void OverrideOrAddAll(PropertyList properties) 
    {
      foreach (var prop in properties)
      {
        AddOrSet(prop.Value, prop.Key);
      }
    }
    public void Remove(string name) => Data.RemoveAll(item => item.Key == name);
    public List<Property>.Enumerator GetEnumerator() { return Data.GetEnumerator(); }
  }
}
