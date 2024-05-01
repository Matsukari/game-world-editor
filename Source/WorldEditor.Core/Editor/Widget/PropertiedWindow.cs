using ImGuiNET;

namespace Raven.Widget
{

  public class PropertiedWindow : Window, IPropertied, IImGuiRenderable
  {
    public virtual PropertyList Properties { get; set; } = new PropertyList();
    public virtual string Name { get; set; } = "";
    public PropertiedWindow() => Flags = ImGuiWindowFlags.NoFocusOnAppearing;

    public override void OnRender(ImGuiWinManager imgui) 
    {
      var name = Name;

      OnRenderBeforeName(imgui);
      if (ImGui.InputText("Name", ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue)) 
      {
        OnChangeName(Name, name);
        Name = name;
      }
      OnRenderAfterName(imgui);

      PropertiesRenderer.Render(imgui, this, OnChangeProperty);
    }
    public bool HasName() => Name != null && Name != string.Empty;

    protected virtual void OnChangeProperty(string name) 
    {
    }
    protected virtual void OnRenderBeforeName(ImGuiWinManager imgui) 
    {
    }
    protected virtual void OnRenderAfterName(ImGuiWinManager imgui) 
    {
    }
    protected virtual void OnChangeName(string prev, string curr) 
    {
    }

  }
}
