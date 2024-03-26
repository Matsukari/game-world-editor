using ImGuiNET;
using Nez;

namespace Raven.Widget
{
  public class Window : IImGuiRenderable
  {
    public string Name;
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public RectangleF Bounds = new RectangleF();

    public virtual void OnRender(Editor editor)
    {
    }
    public virtual void OnHovered() 
    {
    }
    public virtual void Render(Editor editor)
    {
      ImGui.Begin(Name, Flags);
      if (ImGui.IsWindowHovered()) ImGui.SetWindowFocus();
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();
      OnRender(editor);
      ImGui.End();
    }
  }
}
