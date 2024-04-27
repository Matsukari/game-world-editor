using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Widget
{
  public class Window : IImGuiRenderable
  {
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public bool IsOpen 
    { 
      get => _isOpen; 
      set 
      { 
        if (value == _isOpen) return;
        _isOpen = value;
        if (_isOpen && OnOpen != null) OnOpen();
        else if (OnClose != null) OnClose();
      }
    }
    public RectangleF Bounds = new RectangleF();
    public virtual bool CanOpen => true;
    public bool NoClose = true;
    public Vector2 SchedWindowSize = Vector2.Zero;
    public Vector2 SchedWindowPos = Vector2.Zero;

    protected bool _isOpen = true;

    public event Action<ImGuiWinManager> RenderAttachments;
    public event Action OnClose;
    public event Action OnOpen;

    public virtual string GetIcon() => "";

    public virtual string GetName() => GetType().Name;

    public virtual void OnRender(ImGuiWinManager imgui) {}

    public virtual void OnHovered(ImGuiWinManager imgui) {}

    public virtual void OutRender(ImGuiWinManager imgui) {}

    public virtual void InterpretRenderAttachments(ImGuiWinManager imgui) 
    {
      if (RenderAttachments != null)
        RenderAttachments(imgui);
    }

    public virtual void Render(ImGuiWinManager imgui)
    {
      if (!IsOpen || !CanOpen) return;

      if (NoClose) 
        ImGui.Begin(GetName(), Flags);
      else 
        ImGui.Begin(GetName(), ref _isOpen, Flags);

      if (SchedWindowSize != Vector2.Zero) 
      {
        ImGui.SetWindowSize(SchedWindowSize.ToNumerics());
        SchedWindowSize = Vector2.Zero;
      }
      if (SchedWindowPos != Vector2.Zero) 
      {
        ImGui.SetWindowPos(SchedWindowPos.ToNumerics());
        SchedWindowPos = Vector2.Zero;
      }
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();

      if (ImGui.IsWindowHovered()) 
      {
        OnHovered(imgui);
        ImGui.SetWindowFocus();
      }
      OnRender(imgui);
      ImGui.End();  

      OutRender(imgui);

      InterpretRenderAttachments(imgui);
    }
  }
}
