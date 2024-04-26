
namespace Raven
{
  public class WindowHolder : Widget.PropertiedWindow
  {
    public override string Name { get => Content.Name; set => Content.Name = value;}
    public override PropertyList Properties { get => Content.Properties; set => Content.Properties = value; }
    public Widget.PropertiedWindow Content;
    public override bool CanOpen => base.CanOpen;
        
    string _windowName;

    public WindowHolder(string name) => _windowName = name;

    public override string GetName() => _windowName;
        
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (Content != null && Content.CanOpen && Content.IsOpen)
        Content.OnRender(imgui);
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