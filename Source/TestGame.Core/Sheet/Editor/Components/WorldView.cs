
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
    List<Guidelines.GridLines> _grids = new List<Guidelines.GridLines>();
    public bool IsEditFree = false;
    public override void OnChangedTab()
    {
      Position = Vector2.Zero;
      RemoveAllComponents(); 
      AddComponent(new Renderable());
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());

      var origin = AddComponent(new Guidelines.OriginLines());
      origin.Color = Editor.ColorSet.SpriteRegionActiveOutline;

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
        for (var i = 0; i < World.Levels.Count(); i++)
        {
          var level = World.Levels[i];

          batcher.DrawRect(level.Bounds, Editor.ColorSet.LevelSheet);
          var selection = Editor.GetSubEntity<Selection>();
          if (level.Bounds.Contains(camera.MouseToWorldPoint()) && Nez.Input.LeftMouseButtonReleased && World.SelectedSprite == null)
          {
            selection.Begin(level.Bounds, this);
          }
          if (selection.Capture is Renderable)
          {
            level.Position = selection.Bounds.Location;
            level.Size = selection.Bounds.Size.ToPoint();
          }
          foreach (var layer in level.Layers)
          {
            layer.Draw(batcher, camera);
          }
        }
      }
    }
  }
}
