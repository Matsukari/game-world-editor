
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.ImGuiTools;
using Nez;
using Nez.Sprites;
using ImGuiNET;

namespace Raven.Sheet
{
  public class WorldView : Editor.WorldEntity
  {
    public override void OnChangedTab()
    {
      Position = Vector2.Zero;
      RemoveAllComponents(); 
      AddComponent(new Renderable());
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());
    }    
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    }
    public void RenderImGui()
    {
      // Options
    }
    public override void Update()
    {
      base.Update();
    } 
    public class Renderable : Editor.WorldEntity.Renderable<WorldView>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        foreach (var level in World.Levels)
        {
          batcher.DrawRect(level.Bounds, Editor.ColorSet.LevelSheet);
          foreach (var layer in level.Layers)
          {
            layer.Draw(batcher, camera);
          }
        }
      }
    }
  }
}
