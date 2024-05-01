
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class WorldView : ContentView
  {
    public override IImGuiRenderable ImGuiHandler => _imgui;
    public override IInputHandler InputHandler => _input;
    public SpritePicker SpritePicker { get => _imgui.SpritePicker; }
    public WorldViewImGui Window { get => _imgui; }
    public World World { get => _world; }
 
    WorldViewInputHandler _input;
    WorldViewImGui _imgui;

    CommandManager _commandManager;

    World _world;

    // Settings
    public bool IsRandomPaint = false;
    public PaintMode PaintMode = PaintMode.Pen;
    public PaintType PaintType = PaintType.Single;
    Vector2 _initialScale = new Vector2();
    Vector2 _initialPos = new Vector2();

    public bool CanPaint { get => _imgui.SpritePicker.SelectedSprite != null; }

    public override bool CanDealWithType(object content) => content is World;

    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);

      _commandManager = new CommandManager();

      _input = new WorldViewInputHandler(this);
      _input.OnLeftClickScene += OnLeftClickScene;
      _input.OnLeftClickLevel += OnLeftClickLevel;
      _input.OnRightClickLevel += OpenLevelOptions;
      _input.OnRightClickWorld += (position) =>_imgui.Popups.OpenWorldOptions(World);
      _input.Initialize(editor, content);

      _input.OnLeftClickLevel += (level, i) => _imgui.SceneInstanceInspector.Scene = null;
      _input.OnRightClickLevel += (level, i) => _imgui.SceneInstanceInspector.Scene = null;
      _input.OnRightClickWorld += (position) => _imgui.SceneInstanceInspector.Scene = null;

      _imgui = new WorldViewImGui(_input.Painter); 
      _imgui.Initialize(editor, content);
      _imgui.Popups.Initialize(editor, content);
      _imgui.Popups.OnDeleteLevel += level => _imgui.SelectedLevel = -1;
      _imgui.Popups.OnDeleteLevel += level => Selection.End();
      _imgui.Popups.OnCutLevel += level => Selection.End();
      _imgui.SpritePicker.OnLeave += () => { if (CanPaint) Selection.End(); };
      _imgui.SceneInstanceInspector.OnSceneModified += ReSelect;
    }
    public void ReSelect(SpriteSceneInstance instance, FreeformLayer layer)
    {
      if (Selection.Capture is SpriteSceneInstance scene && scene.Name == instance.Name)
        OnLeftClickScene(layer, instance);
    }

    public override void OnContentOpen(IPropertied content)
    {
      _world = content as World; 
      Core.GetGlobalManager<CommandManagerHead>().Current = _commandManager;
    }  
    void OnLeftClickScene(Layer layer, SpriteSceneInstance instance)
    {
      Selection.Begin(instance.ContentBounds.AddPosition(layer.Bounds.Location), instance);
      _initialScale = instance.Props.Transform.Scale;
      _initialPos = instance.Props.Transform.Position;
      _imgui.SceneInstanceInspector.Scene = instance;
      _imgui.SceneInstanceInspector.Layer = layer as FreeformLayer;
      _imgui._objHolder.Content = _imgui.SceneInstanceInspector;
    }
    void OnLeftClickLevel(Level level, int i)
    {
      if (!CanPaint) Selection.Begin(level.Bounds, level);
      _imgui.SelectedLevel = i;
      if (Settings.Graphics.FocusOnOneLevel)
      {
      }
    }
    void OpenLevelOptions(Level level, int i)
    {
      _imgui.Popups.OpenLevelOptions(level);
    }

    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      Guidelines.OriginLinesRenderable.Render(batcher, camera, settings.Colors.OriginLineX.ToColor(), settings.Colors.OriginLineY.ToColor());

      var enterLayer = false;
      for (var i = 0; i < _world.Levels.Count(); i++)
      {
        var level = _world.Levels[i];
        if (!level.IsVisible) continue;

        batcher.DrawRect(level.Bounds, settings.Colors.LevelSheet.ToColor());
        foreach (var layer in level.Layers)
        {
          bool mouseInLayer = layer.Bounds.Contains(Camera.MouseToWorldPoint());
          Color color = default;
          if (settings.Graphics.HighlightCurrentLayer 
              && mouseInLayer
              && CanPaint
              && !InputManager.IsImGuiBlocking
              && _imgui.SelectedLevelInspector != null
              && _imgui.SelectedLevelInspector.Level.Name == level.Name
              && _imgui.SelectedLevelInspector.CurrentLayer != null 
              && _imgui.SelectedLevelInspector.CurrentLayer.Name != layer.Name)
          {
            color = Color.Gray;
          }
          WorldRenderer.RenderLayer(batcher, camera, layer, color);

          if (mouseInLayer && !Selection.HasBegun() && Nez.Input.LeftMouseButtonReleased)
          {
            enterLayer = true;
            _imgui.SelectedLevel = i;
          }

          if (layer is TileLayer tileLayer 
              && _imgui.SelectedLevelInspector != null
              && _imgui.SelectedLevelInspector.Level.Name == level.Name
              && _imgui.SelectedLevelInspector.CurrentLayer != null 
              && _imgui.SelectedLevelInspector.CurrentLayer.Name == layer.Name
              && layer.IsVisible 
              && Settings.Graphics.DrawLayerGrid)
          {
            Guidelines.GridLines.RenderGridLines(batcher, camera, tileLayer.Bounds.Location, settings.Colors.LevelGrid.ToColor(), 
              tileLayer.TilesQuantity, tileLayer.TileSize.ToVector2());
          }
        }
      }
      if (!enterLayer)
      {
      }

      // if (_imgui.SelectedLevelInspector != null && _imgui.SelectedLevelInspector.CurrentLayer is TileLayer tileLayer)
      
      if (Selection.Capture is Level lev)
      {
        lev.LocalOffset = Selection.ContentBounds.Location;
        lev.ContentSize = Selection.ContentBounds.Size.ToPoint();
        if (Input.LeftMouseButtonPressed && !InputManager.IsImGuiBlocking) _startLevel = lev.LocalOffset;
        if (Input.LeftMouseButtonReleased && !InputManager.IsImGuiBlocking) 
        {
          var command = new LevelMoveCommand(lev, _startLevel); 
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(command, ()=>Selection.Re(command._level.Bounds, command._level));
        }
      }
      else if (Selection.Capture is ShapeModel model)
      {
        var bounds = model.Bounds;
        bounds.Location = Selection.ContentBounds.Location;
        bounds.Size = Selection.ContentBounds.Size;
        model.Bounds = bounds;
      }
      else if (Selection.Capture is SpriteSceneInstance instance)
      {
        var scaleDelta = (Selection.ContentBounds.Size - Selection.InitialBounds.Size) / (instance.Scene.Bounds.Size);
        instance.Props.Transform.Position = _initialPos + (Selection.ContentBounds.Location - Selection.InitialBounds.Location) 
          + instance.Scene.MaxOrigin * scaleDelta;
        instance.Props.Transform.Scale = _initialScale + scaleDelta;
      }
      if (_imgui.SelectedLevel != -1)
      {
        batcher.DrawRectOutline(camera, _imgui.SelectedLevelInspector.Level.Bounds, settings.Colors.LevelSelOutline.ToColor());
      }
    }
    Vector2 _startLevel = Vector2.Zero;
    Vector2 _startScene = Vector2.Zero;
  }
}
