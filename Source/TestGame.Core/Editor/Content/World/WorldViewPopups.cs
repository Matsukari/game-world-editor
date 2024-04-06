
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven 
{
  public class WorldViewPopup : EditorInterface, IImGuiRenderable
  {
    public Layer Layer;
    World _world { get => Content as World; }

    World _worldOnOpt = null;
    Level _levelOnOpt = null;
    bool _isOpenWorldOptions = false;
    bool _isOpenLevelOptions = false;
    Vector2 _mouseWhenLevelAdd = Vector2.Zero;

    public event Action<Level> OnDeleteLevel;

    public void Update(Layer layer) => Layer = layer;

    public void OpenWorldOptions(World world) 
    {
      _worldOnOpt = world;
      _isOpenWorldOptions = true;
    }
    public void OpenLevelOptions(Level level) 
    {
      _levelOnOpt = level;
      _isOpenLevelOptions = true;
    }
    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      if (Layer == null) return;

      ImGui.SetNextWindowPos(new System.Numerics.Vector2(263f, 93));
      ImGui.Begin("world-scene-indicator-overlay", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration 
          | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav);

      var scene = new List<string>();
      scene.Add(Layer.Level.World.Name);
      scene.Add($" > {Layer.Level.Name}");
      scene.Add($" > {Layer.Name}");
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
          _levelOnOpt.DetachFromWorld();
          if (OnDeleteLevel != null) OnDeleteLevel(_levelOnOpt);
        }
        ImGui.EndPopup();
      }
      if (ImGui.BeginPopup("world-options-popup"))
      {
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + "  Add level here"))
        {
          _mouseWhenLevelAdd = Camera.MouseToWorldPoint();
          imgui.NameModal.Open((name)=> _world.CreateLevel(name).LocalOffset = _mouseWhenLevelAdd );
        }
        ImGui.EndPopup();
      }

    }
  }
}
