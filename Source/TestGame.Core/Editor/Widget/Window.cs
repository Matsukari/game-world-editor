using ImGuiNET;
using Nez;

namespace Raven.Widget
{
  public class Window : IImGuiRenderable
  {
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public RectangleF Bounds = new RectangleF();
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
    public virtual bool CanOpen => true;
    public bool NoClose = true;

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

      if (ImGui.IsWindowHovered()) 
      {
        OnHovered(imgui);
        ImGui.SetWindowFocus();
      }
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();
      OnRender(imgui);
      ImGui.End();  

      OutRender(imgui);

      InterpretRenderAttachments(imgui);
    }
  }
}
