
namespace Raven
{
  public class SceneInstanceInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Scene.Name; set => Scene.Name = value;}
    public override PropertyList Properties { get => Scene.Properties; set => Scene.Properties = value; }
    public override bool CanOpen => Scene != null && Layer != null;
        
    public SpriteSceneInstance Scene;
    public FreeformLayer Layer;
    public event Action<SpriteSceneInstance, FreeformLayer> OnSceneModified;

    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      if (Scene.Props.Transform.RenderImGui() && OnSceneModified != null) OnSceneModified(Scene, Layer);
    } 
  }
}
