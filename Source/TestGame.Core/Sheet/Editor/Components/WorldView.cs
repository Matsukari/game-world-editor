
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
      AddComponent(new Utils.Components.CameraMoveComponent());
      AddComponent(new Utils.Components.CameraZoomComponent());

      var origin = AddComponent(new Guidelines.OriginLines());
      origin.Color = Editor.ColorSet.SpriteRegionActiveOutline;
      AddComponent(new Renderable());

    }    
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    }
    Vector2 _mouseWhenLevelAdd = Vector2.Zero;
    public void RenderImGui()
    {
      if (!Enabled) return;

      ImGui.SetNextWindowPos(new System.Numerics.Vector2(263f, 93));
      ImGui.Begin("world-scene-indicator-overlay", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration 
          | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav);
    
      void BreadCrumb(params string[] labels)
      {
        ImGui.PushStyleColor(ImGuiCol.Text, GuiColors.Get(ImGuiCol.TextDisabled));
        for (int i = 0; i < labels.Count()-1; i++)
        {
          ImGui.Text(labels[i]);
          ImGui.SameLine();
        }
        ImGui.PopStyleColor();
        ImGui.Text(labels.Last());
      }
      var scene = new List<string>();
      scene.Add(World.Name);
      if (World.CurrentLevel != null)
      {
        scene.Add($" > {World.CurrentLevel.Name}");
        if (World.CurrentLevel.CurrentLayer != null)
        {
          scene.Add($" > {World.CurrentLevel.CurrentLayer.Name}");
        }
      }
      BreadCrumb(scene.ToArray());
      ImGui.End();

      if (_isOpenWorldOptinos)
      {
        _isOpenWorldOptinos = false;
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
          _mouseWhenLevelAdd = Scene.Camera.MouseToWorldPoint();
          Editor.OpenNameModal((name)=>{ World.CreateLevel(name).LocalOffset = _mouseWhenLevelAdd; });
        }
        ImGui.EndPopup();
      }

    }
    public override void Update()
    {
      base.Update();
      
    } 
    bool _isOpenWorldOptinos = false;
    bool _isOpenLevelOptions = false;
    public class Renderable : Editor.WorldEntity.Renderable<WorldView>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        var selection = Editor.GetSubEntity<Selection>();
        var input = Core.GetGlobalManager<Raven.Input.InputManager>();
        for (var i = 0; i < WorldGui._levelGuis.Count(); i++)
        {
          var level = World.Levels[i];
          var levelGui = WorldGui._levelGuis[i];
          if (!level.Enabled) continue;

          batcher.DrawRect(level.Bounds, Editor.ColorSet.LevelSheet);
  
          if (Nez.Input.LeftMouseButtonPressed && !input.IsImGuiBlocking)
          {
            if (level.Bounds.Contains(camera.MouseToWorldPoint()) && WorldGui.SelectedSprite == null) 
            {
              selection.Begin(level.Bounds, level);
              WorldGui.SelectedLevel = i;
            }
          }
          else if (Nez.Input.RightMouseButtonPressed && !input.IsImGuiBlocking)
          {
            if (level.Bounds.Contains(camera.MouseToWorldPoint())) 
            {
              if (WorldGui.SelectedSprite == null) Parent._isOpenLevelOptions = true;
            }
            else Parent._isOpenWorldOptinos = true;
            
          }
          level.Render(batcher, camera);
          levelGui.Render(batcher, camera);
        }
        if (selection.Capture is Level lev)
        {
          lev.LocalOffset = selection.Bounds.Center;
          lev.ContentSize = selection.Bounds.Size.ToPoint();
        }
        World.DrawArtifacts(batcher, camera, Editor, Gui);
      }
    }
  }
}
