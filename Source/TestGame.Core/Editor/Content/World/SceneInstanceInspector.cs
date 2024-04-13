
namespace Raven
{
  public class SceneInstanceInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Scene.Name; set => Scene.Name = value;}
    public override PropertyList Properties { get => Scene.Properties; set => Scene.Properties = value; }
    public SpriteSceneInstance Scene;
    public FreeformLayer Layer;
    public event Action<SpriteSceneInstance, FreeformLayer> OnSceneModified;

    public override void Render(ImGuiWinManager imgui)
    {
      if (Scene != null && Layer != null)
        base.Render(imgui);
    }
    protected override void OnRenderAfterName()
    {
      if (Scene.Props.Transform.RenderImGui() && OnSceneModified != null) OnSceneModified(Scene, Layer);
    } 
  }
}
