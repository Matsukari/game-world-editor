using ImGuiNET;

namespace Raven
{
  public class PropertyList
  {
    Dictionary<string, object> _props = new Dictionary<string, object>();
    int _counter = 0;
    public void Add<T>(T obj)
    {
      string name = $"{obj.GetType().Name}.{_counter++}";
      if (obj is IPropertied prop) 
      {
        if (prop.Name != "") name = prop.Name;
      }
      _props.TryAdd(name, obj);
    }
    public void Remove(string name) => _props.Remove(name);
    public Dictionary<string, object>.Enumerator GetEnumerator() { return _props.GetEnumerator(); }
  }
  public interface IPropertied
  {
    PropertyList Properties { get; }
    string Name { get; set; }
    void RenderImGui();
  }
  public class Propertied : IPropertied
  {
    public PropertyList Properties { get; set; }
    public string Name { get; set; }
    public void RenderImGui()
    {
      var name = "";
      ImGui.Begin(GetType().Name);
      if (ImGui.InputText("Name", ref name, 10)) Name = name;
      RenderProperties();
      ImGui.End();
    }
    protected virtual void OnCreateProperty(string name) {}
    protected void RenderProperties()
    {
      
    }
  }
  public enum CustomPropertyType 
  {
    STRING,
    FILE,
    COLOR,
    INT,
    FLOAT,
    BOOL,
    VECTOR2
  };

}
