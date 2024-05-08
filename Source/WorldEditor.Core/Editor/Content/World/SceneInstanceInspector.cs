using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

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
    
    static bool _mod = false;
    static RenderProperties _startProps;
    static void StartProps(SpriteSceneInstance sprite)
    {
      _mod = true;
      if (sprite.Props == null) _startProps = null;
      else _startProps = sprite.Props.Copy(); 
    }
    bool _onRelease = false;
    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      if (Scene.Props.Transform.RenderImGui()) 
      {
        StartProps(Scene);
        if (OnSceneModified != null) 
          OnSceneModified(Scene, Layer);
      }

      var color = Scene.Props.Color.ToNumerics();
      if (ImGui.ColorEdit4("Tint", ref color)) 
      {
        _onRelease = true;
        StartProps(Scene);
        Scene.Props.Color = color;
      }

      var flipBoth = Scene.Props.SpriteEffects == (SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
      var flipH = Scene.Props.SpriteEffects == SpriteEffects.FlipHorizontally || flipBoth;
      var flipV = Scene.Props.SpriteEffects == SpriteEffects.FlipVertically || flipBoth;
      if (ImGui.Checkbox("Flip X", ref flipH)) 
      {
        StartProps(Scene);
        Scene.Props.SpriteEffects ^= SpriteEffects.FlipHorizontally;
      }
      ImGui.SameLine();
      if (ImGui.Checkbox("Flip Y", ref flipV)) 
      {
        StartProps(Scene);
        Scene.Props.SpriteEffects ^= SpriteEffects.FlipVertically;
      }
      if (_mod && (!_onRelease || Nez.Input.LeftMouseButtonReleased))
      {
        _onRelease = false;
        _mod = false;
        Nez.Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RenderPropModifyCommand(Scene, "Props", Scene.Props, _startProps));
      }
    }
  }
}
