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
    public static void RenderProperties(IPropertied propertied)
    {
    }
  }
  public class Propertied : IPropertied
  {
    public PropertyList Properties { get; set; }
    public string Name { get; set; } = "";
    public virtual void RenderImGui()
    {
      var name = Name;
      ImGui.Begin(GetType().Name);
      if (ImGui.IsWindowHovered()) ImGui.SetWindowFocus();
      if (ImGui.InputText("Name", ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue)) Name = name;
      if (ImGui.GetIO().MouseClicked[1] && ImGui.IsWindowFocused()) ImGui.OpenPopup("prop-popup");
      if (ImGui.BeginPopupContextItem("prop-popup"))
      {
        ImGui.Text("New Property"); 
          ImGui.Separator();
          ImGui.Indent();
          if (ImGui.MenuItem("String")) Console.WriteLine("String");
          if (ImGui.MenuItem("Boolean")) Console.WriteLine("String");
          if (ImGui.MenuItem("Integer")) Console.WriteLine("String");
          if (ImGui.MenuItem("Float")) Console.WriteLine("String");
          if (ImGui.MenuItem("Vector2")) Console.WriteLine("String");
          ImGui.Unindent();
        ImGui.EndPopup();
      }
      IPropertied.RenderProperties(this);
      ImGui.End();
      if (Name != null && Name != string.Empty) OnCreateProperty(Name);
    }
    protected virtual void OnCreateProperty(string name) {}
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
