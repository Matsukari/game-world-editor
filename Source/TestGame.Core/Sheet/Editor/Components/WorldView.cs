
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
    Vector2 _mouseWhenLevelAdd = Vector2.Zero;
    public void RenderImGui()
    {
      if (!Enabled) return;
      for (var i = 0; i < World.Levels.Count(); i++)
      {
        var level = World.Levels[i];
        if (Nez.Input.RightMouseButtonReleased)
        {
          if (level.Bounds.Contains(Scene.Camera.MouseToWorldPoint()))
          {
            ImGui.OpenPopup("level-options-popup");
          }
          else
          {
            ImGui.OpenPopup("world-options-popup");
          }
        }
      }
      if (ImGui.BeginPopupContextItem("level-options-popup"))
      {
        ImGui.EndPopup();
      }
      if (ImGui.BeginPopupContextItem("world-options-popup"))
      {
        if (ImGui.MenuItem("Add level here"))
        {
          _mouseWhenLevelAdd = Scene.Camera.MouseToWorldPoint();
          Editor.OpenNameModal((name)=>{ World.CreateLevel(name).Position = _mouseWhenLevelAdd; });
        }
        ImGui.EndPopup();
      }

    }
    public override void Update()
    {
      base.Update();
      
    } 
    public class Renderable : Editor.WorldEntity.Renderable<WorldView>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        var selection = Editor.GetSubEntity<Selection>();
        for (var i = 0; i < World.Levels.Count(); i++)
        {
          var level = World.Levels[i];

          batcher.DrawRect(level.Bounds, Editor.ColorSet.LevelSheet);
  
          if (Nez.Input.LeftMouseButtonReleased)
          {
            if (level.Bounds.Contains(camera.MouseToWorldPoint()) && World.SelectedSprite == null) selection.Begin(level.Bounds, level);
          }
          foreach (var layer in level.Layers)
          {
            layer.Draw(batcher, camera);
          }
        }
        if (selection.Capture is World.Level lev)
        {
          lev.Position = selection.Bounds.Location;
          lev.Size = selection.Bounds.Size.ToPoint();
        }
        World.DrawArtifacts(batcher, camera, Editor, Gui);
      }
    }
  }
}
