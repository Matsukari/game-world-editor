
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
  public class WorldViewInputHandler : EditorInterface, IInputHandler
  {
    readonly WorldView _view;
    public event Action<LevelEntity, int> OnLeftClickLevel;
    public event Action<LevelEntity, int> OnRightClickLevel;
    public event Action<Vector2> OnRightClickWorld;
    public readonly TilePainter Painter;

    public WorldViewInputHandler(WorldView view)
    {
      _view = view;
      Painter = new TilePainter(_view);
    }

    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      IInputHandler paintInput = Painter;
      if (paintInput.OnHandleInput(input)) return true;

      var selectLevel = false;
      for (var i = 0; i < _view.WorldEntity.Levels.Count(); i++)
      {
        var level = _view.WorldEntity.Levels[i];
        if (!level.Enabled) continue;

        if (Nez.Input.LeftMouseButtonPressed 
            && level.Bounds.Contains(Camera.MouseToWorldPoint()) 
            && _view.SpritePicker.SelectedSprite == null 
            && OnLeftClickLevel != null)
        {
          OnLeftClickLevel(level, i);
          return true;
        }
        else if (Nez.Input.RightMouseButtonPressed)
        {
          if (level.Bounds.Contains(Camera.MouseToWorldPoint()) && OnRightClickLevel != null)
          {
            OnRightClickLevel(level, i);
            selectLevel = true;
            return true;
          }
        }
      }
      if (Nez.Input.RightMouseButtonPressed && !selectLevel && OnRightClickWorld != null)
      {
        OnRightClickWorld(Camera.MouseToWorldPoint());
        return true;
      }
      return false;
    }
  }
  public class WorldView : ContentView
  {
    public override IImGuiRenderable ImGuiHandler => _imgui;
    public override IInputHandler InputHandler => _input;
    public WorldEntity WorldEntity { get => _worldEntity; }
    public SpritePicker SpritePicker { get => _imgui.SpritePicker; }
    public WorldViewImGui Window { get => _imgui; }
 
    WorldViewInputHandler _input;
    WorldViewImGui _imgui;

    World _world { get => _worldEntity.World; }
    WorldEntity _worldEntity;

    // Settings
    public bool IsDrawTileLayerGrid = true;
    public bool IsRandomPaint = false;
    public PaintMode PaintMode = PaintMode.Pen;
    public PaintType PaintType = PaintType.Single;

    public bool CanPaint { get => _imgui.SpritePicker.SelectedSprite != null; }

    public override bool CanDealWithType(object content) => content is World;

    public override void OnContentOpen(IPropertied content)
    {
      _worldEntity = new WorldEntity(content as World); 
      _input = new WorldViewInputHandler(this);
      _input.OnLeftClickLevel += SelectLevel;
      _input.OnRightClickLevel += OpenLevelOptions;

      SpritePicker.HandleSelectedSprite = _input.Painter.HandleSelectedSprite;
    }   
    void SelectLevel(LevelEntity level, int i)
    {
      Selection.Begin(level.Bounds, level.Level);
      _imgui.SelectedLevel = i;
    }
    void OpenLevelOptions(LevelEntity level, int i)
    {
      _imgui.Popups.OpenLevelOptions(level.Level);
    }
    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      Guidelines.OriginLinesRenderable.Render(batcher, camera, settings.Colors.OriginLineX.ToColor(), settings.Colors.OriginLineY.ToColor());
      for (var i = 0; i < _worldEntity.Levels.Count(); i++)
      {
        var level = _worldEntity.Levels[i];
        if (!level.Enabled) continue;

        batcher.DrawRect(level.Bounds, settings.Colors.LevelSheet.ToColor());
        foreach (var layer in level.GetComponents<RenderableComponent>())
        {
          layer.Render(batcher, camera);
        }
      }

      if (_imgui.SelectedLevelInspector.CurrentLayer is TileLayer tileLayer)
        Guidelines.GridLines.RenderGridLines(batcher, camera, tileLayer.Level.Bounds.Location, settings.Colors.LevelGrid.ToColor(), 
            tileLayer.TilesQuantity, tileLayer.TileSize.ToVector2());
      
      if (Selection.Capture is Level lev)
      {
        lev.LocalOffset = Selection.ContentBounds.Center;
        lev.ContentSize = Selection.ContentBounds.Size.ToPoint();
      }
      if (_imgui.SelectedLevel != -1)
      {
        batcher.DrawRectOutline(camera, _imgui.SelectedLevelInspector.Level.Bounds, settings.Colors.LevelSelOutline.ToColor());
      }
    }
  }
}
