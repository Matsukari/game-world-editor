using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
  public class SceneInstanceInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Scene.Name; set => Scene.Name = value;}
    public override PropertyList Properties { get => Scene.Properties; set => Scene.Properties = value; }
    public SpriteScene Scene;

    public override void Render(ImGuiWinManager imgui)
    {
      if (Scene != null)
        base.Render(imgui);
    }
    protected override void OnRenderAfterName()
    {
      Scene.Transform.RenderImGui();
    } 
  }
}
