using ImGuiNET;
using Nez;

namespace Raven.Widget
{
  public class Window : IImGuiRenderable
  {
    public string Name = "";
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public RectangleF Bounds = new RectangleF();
    public bool IsOpen = true;
    public bool NoClose = true;

    public virtual void OnRender(Editor editor)
    {
    }
    public virtual void OnHovered() 
    {
    }
    public virtual void Render(Editor editor)
    {
      if (NoClose) ImGui.Begin(Name, Flags);
      else if (ImGui.Begin(Name, ref IsOpen, Flags))
      {
        if (ImGui.IsWindowHovered()) 
        {
          OnHovered();
          ImGui.SetWindowFocus();
        }
        Bounds.Location = ImGui.GetWindowPos();
        Bounds.Size = ImGui.GetWindowSize();
        OnRender(editor);
        ImGui.End();  
      }
    }
  }
}
