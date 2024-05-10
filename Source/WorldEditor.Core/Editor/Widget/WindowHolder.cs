
namespace Raven
{
  public class WindowHolder : Widget.PropertiedWindow
  {
    public override string Name { get => Content.Name; set => Content.Name = value;}
    public override PropertyList Properties { get => Content.Properties; set => Content.Properties = value; }
    public Widget.PropertiedWindow Content;
        
    string _windowName;

    public WindowHolder(string name)
    {
      _windowName = name;
      Flags |= ImGuiNET.ImGuiWindowFlags.AlwaysVerticalScrollbar;

    } 

    public override string GetName() => _windowName;

    public override void InterpretRenderAttachments(ImGuiWinManager imgui) 
    {
      if (Content != null && Content.CanOpen && Content.IsOpen)
        Content.InterpretRenderAttachments(imgui);
    }
        
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (Content != null && Content.CanOpen && Content.IsOpen)
      {
        Content.OnRender(imgui);
      }
      else 
      {
        ImGuiUtils.TextMiddle("No object selected");
      }
    }
    public override void OutRender(ImGuiWinManager imgui) 
    {
      if (Content != null && Content.CanOpen && Content.IsOpen)
        Content.OutRender(imgui);
    }

  }
}
