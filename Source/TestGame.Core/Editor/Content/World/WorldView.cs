
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven
{
  public class WorldView : ContentView
  {
    World _world;
    public bool IsEditFree = false;
    public override void OnContent()
    {
      Entity.Scene.EntitiesOfType<World>().ForEach((world)=>world.Enabled=false);
      if (RestrictTo<World>())
      {
        _world = Content as World;
        _world.Enabled = true;
        Entity.GetComponent<Utils.Components.CameraMoveComponent>().Enabled = true;
        Entity.GetComponent<Utils.Components.CameraZoomComponent>().Enabled = true;
      }
    }    
    Vector2 _mouseWhenLevelAdd = Vector2.Zero;
    public void Render(Editor editor)
    {
      if (!Enabled) return;

      ImGui.SetNextWindowPos(new System.Numerics.Vector2(263f, 93));
      ImGui.Begin("world-scene-indicator-overlay", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration 
          | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav);
    
      var scene = new List<string>();
      scene.Add(_world.Name);
      if (_world.CurrentLevel != null)
      {
        scene.Add($" > {_world.CurrentLevel.Name}");
        if (_world.CurrentLevel.CurrentLayer != null)
        {
          scene.Add($" > {_world.CurrentLevel.CurrentLayer.Name}");
        }
      }
      Widget.ImGuiWidget.BreadCrumb(scene.ToArray());
      ImGui.End();

      if (_isOpenWorldOptions)
      {
        _isOpenWorldOptions = false;
        ImGui.OpenPopup("world-options-popup");
      }
      if (_isOpenLevelOptions)
      {
        _isOpenLevelOptions = false;
        ImGui.OpenPopup("level-options-popup");
      }

      // Popups
      if (ImGui.BeginPopup("level-options-popup"))
      {
        if (ImGui.MenuItem(IconFonts.FontAwesome5.Trash + "  Delete")) 
        {
          
        }
        ImGui.EndPopup();
      }
      if (ImGui.BeginPopup("world-options-popup"))
      {
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + "  Add level here"))
        {
          _mouseWhenLevelAdd = Entity.Scene.Camera.MouseToWorldPoint();
          Editor.NameModal.Open((name)=> _world.CreateLevel(name).LocalOffset = _mouseWhenLevelAdd );
        }
        ImGui.EndPopup();
      }

    }
    public override void Update()
    {
      base.Update(); 
    } 
    bool _isOpenWorldOptions = false;
    bool _isOpenLevelOptions = false;
    public override void Render(Batcher batcher, Camera camera)
    {
      Guidelines.OriginLinesRenderable.Render(batcher, camera, Editor.Settings.Colors.OriginLineX.ToColor(), Editor.Settings.Colors.OriginLineY.ToColor());
      var worldEditor = Editor.GetEditorComponent<WorldEditor>();
      var selection = Editor.GetEditorComponent<Selection>();
      var input = Core.GetGlobalManager<Raven.Input.InputManager>();
      for (var i = 0; i < worldEditor._levelInspectors.Count(); i++)
      {
        var level = _world.Levels[i];
        var levelInspector = worldEditor._levelInspectors[i];
        if (!level.Enabled) continue;

        batcher.DrawRect(level.Bounds, Editor.Settings.Colors.LevelSheet.ToColor());

        if (Nez.Input.LeftMouseButtonPressed && !input.IsImGuiBlocking)
        {
          if (level.Bounds.Contains(camera.MouseToWorldPoint()) && worldEditor.SelectedSprite == null) 
          {
            selection.Begin(level.Bounds, level);
            worldEditor.SelectedLevel = i;
          }
        }
        else if (Nez.Input.RightMouseButtonPressed && !input.IsImGuiBlocking)
        {
          if (level.Bounds.Contains(camera.MouseToWorldPoint())) 
          {
            if (worldEditor.SelectedSprite == null) _isOpenLevelOptions = true;
          }
          else _isOpenWorldOptions = true;

        }
        levelInspector.Render(batcher, camera);
      }
      if (selection.Capture is Level lev)
      {
        lev.LocalOffset = selection.ContentBounds.Center;
        lev.ContentSize = selection.ContentBounds.Size.ToPoint();
      }
      if (Editor.GetEditorComponent<WorldEditor>().SelectedLevel != -1)
      {
        batcher.DrawRectOutline(camera, _world.CurrentLevel.Bounds, Editor.Settings.Colors.LevelSelOutline.ToColor());
      }
    }
  }
}
