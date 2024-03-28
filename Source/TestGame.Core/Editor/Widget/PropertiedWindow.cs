using ImGuiNET;

namespace Raven.Widget
{
  public class PropertiedWindow : Window, IPropertied, IImGuiRenderable
  {
    public virtual PropertyList Properties { get; set; } = new PropertyList();
    public virtual new string Name { get; set; } = "";
    public PropertiedWindow() => Flags = ImGuiWindowFlags.NoFocusOnAppearing;

    public override void Render(Editor editor)
    {
      var name = Name;

      void Draw()
      {
        if (ImGui.IsWindowHovered()) ImGui.SetWindowFocus();
        Bounds.Location = ImGui.GetWindowPos();
        Bounds.Size = ImGui.GetWindowSize();
        OnRender(editor);

        OnRenderBeforeName();
        if (ImGui.InputText("Name", ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue)) 
        {
          OnChangeName(Name, name);
          Name = name;
        }
        OnRenderAfterName();
        if (PropertiesRenderer.Render(editor, this)) OnChangeProperty(name);

        if (PropertiesRenderer.HandleNewProperty(this, editor)) OnChangeProperty(name);
      }
      var windowname = GetIcon() + "   " + GetName();
      if (NoClose) 
      {
        ImGui.Begin(windowname, Flags);
        Draw();
        ImGui.End();
      }
      else if (IsOpen)
      {
        ImGui.Begin(windowname, ref IsOpen, Flags);
        Draw();
        ImGui.End();
      }
    }
    public bool HasName() => Name != null && Name != string.Empty;
    public virtual string GetIcon() => "";
    public virtual string GetName() => GetType().Name;
    protected virtual void OnChangeProperty(string name) 
    {
    }
    protected virtual void OnRenderBeforeName() 
    {
    }
    protected virtual void OnRenderAfterName() 
    {
    }
    protected virtual void OnChangeName(string prev, string curr) 
    {
    }

  }
}
