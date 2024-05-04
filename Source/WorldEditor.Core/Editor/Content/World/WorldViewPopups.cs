
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven 
{
  public class WorldViewPopup : EditorInterface, IImGuiRenderable
  {
    public Layer Layer;
    World _world { get => Content as World; }

    World _worldOnOpt = null;
    Level _levelOnOpt = null;
    Level _copiedLevel = null;
    Level _cutLevel = null;
    bool _isOpenWorldOptions = false;
    bool _isOpenLevelOptions = false;
    Vector2 _mouseWhenLevelAdd = Vector2.Zero;
    public Widget.PopupDelegate<(IPropertied, string, ShapeModel)> ShapePopup = new Widget.PopupDelegate<(IPropertied, string, ShapeModel)>("shape-popup");

    public event Action<ImGuiWinManager> OnAnyPopupRender;
    public event Action<Level> OnDeleteLevel;
    public event Action<Level> OnCutLevel;
    public event Action<Level> OnCopyLevel;
    public event Action<Level> OnPasteLevel;

    public void Update(Layer layer) => Layer = layer;

    public void OpenWorldOptions(World world) 
    {
      _worldOnOpt = world;
      _isOpenWorldOptions = true;
      Insist.IsNotNull(_worldOnOpt);
    }
    public void OpenLevelOptions(Level level) 
    {
      _levelOnOpt = level;
      _isOpenLevelOptions = true;
      Insist.IsNotNull(_levelOnOpt);
    }
    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      ShapePopup.Render(imgui);
      if (Layer != null) 
      {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(263f, 93));
        ImGui.Begin("world-scene-indicator-overlay", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration 
            | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav);

        var scene = new List<string>();
        scene.Add(Layer.Level.World.Name);
        scene.Add($" > {Layer.Level.Name}");
        scene.Add($" > {Layer.Name}");
        Widget.ImGuiWidget.BreadCrumb(scene.ToArray());
        ImGui.End();
      }

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
        if (OnAnyPopupRender != null) OnAnyPopupRender(imgui);

        if (ImGui.MenuItem(Icon.Copy + "  Copy")) 
        {
          _copiedLevel = _levelOnOpt;
          _levelOnOpt = null;
        }
        if (ImGui.MenuItem(Icon.Copy + "  Cut")) 
        {
          _copiedLevel = null;
          _levelOnOpt.DetachFromWorld();
          if (OnCutLevel != null) OnCutLevel(_levelOnOpt);
        }
        if (ImGui.MenuItem(Icon.Trash + "  Delete")) 
        {
          _levelOnOpt.DetachFromWorld();
          if (OnDeleteLevel != null) OnDeleteLevel(_levelOnOpt);
        }
        ImGui.EndPopup();
      }
      if (ImGui.BeginPopup("world-options-popup"))
      {
        if (OnAnyPopupRender != null) OnAnyPopupRender(imgui);

        if (ImGui.MenuItem(Icon.ArrowDown + "  Add level here"))
        {
          _mouseWhenLevelAdd = Camera.MouseToWorldPoint();
          imgui.NameModal.Open((name)=> _world.CreateLevel(name).LocalOffset = _mouseWhenLevelAdd );
        }
        if ((_copiedLevel != null || _levelOnOpt != null) && ImGui.MenuItem(Icon.Paste + "  Paste"))
        {
          Level level;
          if (_copiedLevel != null) level = _copiedLevel.Copy();
          else 
          {
            level = _levelOnOpt;
            _levelOnOpt = null;
          }
          Insist.IsNotNull(level);
          _world.PutLevel(level);
          level.LocalOffset = Camera.MouseToWorldPoint();

          if (OnPasteLevel != null) OnPasteLevel(level);
        }
        ImGui.EndPopup();
      }

    }
  }
}
