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

    public virtual void OnRender(ImGuiWinManager imgui)
    {
    }
    public virtual void OnHovered() 
    {
    }
    public virtual void Render(ImGuiWinManager imgui)
    {
      void Draw()
      {
        if (ImGui.IsWindowHovered()) 
        {
          OnHovered();
          ImGui.SetWindowFocus();
        }
        Bounds.Location = ImGui.GetWindowPos();
        Bounds.Size = ImGui.GetWindowSize();
        OnRender(imgui);
      }
      if (NoClose) 
      {
        ImGui.Begin(Name, Flags);
        Draw();
        ImGui.End();
      }
      else if (IsOpen)
      {
        ImGui.Begin(Name, ref IsOpen, Flags);
        Draw();
        ImGui.End();  
      }
    }
  }
}
