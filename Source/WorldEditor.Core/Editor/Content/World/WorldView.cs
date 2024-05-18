
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class WorldView : ContentView
  {
    public override IImGuiRenderable ImGuiHandler => _imgui;
    public override IInputHandler InputHandler => _input;
    public TilePainter SpritePicker { get => _imgui.SpritePicker; }
    public WorldViewImGui Window { get => _imgui; }
    public World World { get => _world; }
 
    WorldViewInputHandler _input;
    WorldViewImGui _imgui;

    CommandManager _commandManager;

    World _world;

    // Settings
    public bool IsRandomPaint = false;
    public PaintMode PaintMode 
    {
      get => _paintMode;
      set
      {
        _paintMode = value;
        ClearEditTraces();
      }
    }
    public PaintType PaintType 
    {
      get => _paintType;
      set
      {
        _paintType = value;
        ClearEditTraces();
      }
    }

    PaintMode _paintMode = PaintMode.None;
    PaintType _paintType = PaintType.Single;

    public bool CanPaint { get => _paintMode == PaintMode.Pen || PaintMode == PaintMode.Eraser; }

    public override bool CanDealWithType(object content) => content is World;

    void ClearEditTraces()
    {
      Selection.End();  
      SpritePicker._selectedBounds = RectangleF.Empty;
      if (_imgui._objHolder != null)
        _imgui._objHolder.Content = _imgui.SelectedLevelInspector;
    }

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

      Selection.OnScaled += HandleTileLayerResize;
      Selection.OnMoveEnd += SelectionDragEnd;
      Selection.OnMoveEnd += SelectionDragEnd2;
      Selection.OnScaleEnd += SelectionResizeEnd;
      Selection.OnEditPoint += LevelResize;

      _imgui = new WorldViewImGui(_input.Painter); 
      _imgui.Initialize(editor, content);
      _imgui.Popups.Initialize(editor, content);
      _imgui.Popups.OnDeleteLevel += level => _imgui.SelectedLevel = -1;
      _imgui.Popups.OnDeleteLevel += level => Selection.End();
      _imgui.Popups.OnCutLevel += level => Selection.End();
      _imgui.SpritePicker.OnLeave += () => { if (CanPaint) Selection.End(); };
      _imgui.SceneInstanceInspector.OnSceneModified += ReSelect;
      _imgui.SpritePicker.OnSelectTileInstance += SelectTileInstance; 
    }
    void SelectTileInstance(TileInstance tile, TileLayer layer)
    {
      Console.WriteLine("Selected tile e " );
      _imgui.TileInstanceInspector.Tile = tile;
      _imgui.TileInstanceInspector.Layer = layer;
      _imgui._objHolder.Content = _imgui.TileInstanceInspector; 
    }
    public void ReSelect(SpriteSceneInstance instance, FreeformLayer layer)
    {
      if (Selection.Capture is SpriteSceneInstance scene && scene.Name == instance.Name)
        OnLeftClickScene(layer, instance);
    }
    public override void OnContentOpen(ImGuiWinManager imgui)
    {
      Graphics.Instance.Batcher.ShouldRoundDestinations = false;
      try 
      {
        imgui.GetRenderable<WindowHolder>("sub").Content = _imgui.SelectedLevelInspector;
      }
      catch (Exception) {}
    }
    public override void OnContentOpen(IPropertied content)
    {
      _world = content as World; 
      Core.GetGlobalManager<CommandManagerHead>().Current = _commandManager;
      Selection.End();
    }  
    void HandleTileLayerResize(SelectionAxis axis)
    {
      if (Selection.Capture is not Level) return;
      var level = Selection.Capture as Level;
      foreach (var layer in level.Layers)
      {
        if (layer is TileLayer tileLayer)
        {
          if (axis == SelectionAxis.TopLeft)
          {
             
          }
        }
      }
    }
    void SelectionResizeEnd()
    {
      if (Selection.Capture is Level lev)
      {
        var command = new LevelResizeCommand(lev, _startLevelInstance); 
        void OnUndoRedo()
        {
          Selection.Re(command._level.Bounds, command._level);
          _imgui.SelectedLevel = _world.Levels.FindIndex(item => item.Name == command._level.Name);
          _imgui.SelectedLevelInspector.SetCurrentLayer(0);
          _imgui._objHolder.Content = _imgui.SelectedLevelInspector;

        }
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(command, OnUndoRedo);
      }
    }
    void SelectionDragEnd()
    {
      if (Selection.Capture is Level lev)
      {
        var command = new LevelMoveCommand(lev, _startLevel); 
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(command, ()=>Selection.Re(command._level.Bounds, command._level));
      }
    }
    void SelectionDragEnd2()
    {
      if (Selection.Capture is SpriteSceneInstance scene && Selection.HasChanged)
      {
        var command = new RenderPropTransformModifyCommand(scene.Props, _startScene);
        command.Context = scene;
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(command, ()=>Selection.Re(scene.ContentBounds.AddPosition(scene.Layer.Bounds.Location), 
              command.Context, ()=>_startScene=scene.Props.Transform.Copy()));
      }
    }
    void LevelResize(SelectionAxis axis, Vector2 delta)
    {
      if (Selection.Capture is Level level)
      {
        level.ReBounds(Selection.ContentBounds, axis, delta);
      }
    }
    void OnLeftClickScene(Layer layer, SpriteSceneInstance instance)
    {
      if (_imgui.CurrentLayer is not FreeformLayer || CanPaint || PaintMode != PaintMode.Inspector) return;
      Selection.Begin(instance.ContentBounds.AddPosition(layer.Bounds.Location), instance);
      _startScene = instance.Props.Transform.Copy();
      _imgui.SceneInstanceInspector.Scene = instance;
      _imgui.SceneInstanceInspector.Layer = layer as FreeformLayer;
      _imgui._objHolder.Content = _imgui.SceneInstanceInspector;
    }
    void OnLeftClickLevel(Level level, int i)
    {
      if (!CanPaint && PaintMode == PaintMode.None) 
      {
        Selection.Begin(level.Bounds, level);
      }
      else if (Settings.Graphics.FocusOnOneLevel) return;

      Console.WriteLine("Selected level " + level.Name);
      _imgui.SelectedLevel = i;
      _imgui._objHolder.Content = _imgui.SelectedLevelInspector;
      _startLevel = level.LocalOffset;
      _startLevelInstance = level.Copy();
      if (Settings.Graphics.FocusOnOneLevel)
      {
      }
    }
    void OpenLevelOptions(Level level, int i)
    {
      _imgui.Popups.OpenLevelOptions(level);
    }

    void DrawLayers(int index, Batcher batcher, Camera camera)
    {
      var input = Core.GetGlobalManager<InputManager>();
      var level = World.Levels[index]; 
      foreach (var layer in level.Layers)
      {
        if (!layer.IsVisible) continue;

        bool mouseInLayer = layer.Bounds.Contains(Camera.MouseToWorldPoint());
        bool isSameLevel = _imgui.SelectedLevelInspector != null && _imgui.SelectedLevelInspector.Level.Name == level.Name;
        bool isSameLayer = isSameLevel && _imgui.SelectedLevelInspector.CurrentLayer != null && _imgui.SelectedLevelInspector.CurrentLayer.Name == layer.Name;

        Color color = default;
        if (Settings.Graphics.HighlightCurrentLayer 
            && isSameLevel
            && (!Settings.Graphics.HighlightLayerOnlyIfMouseIsInside || mouseInLayer || SpritePicker.IsDoingWork)
            && PaintMode != PaintMode.None
            && !isSameLayer
            && !InputManager.IsImGuiBlocking)
        {
          color = Settings.Colors.LayerDim.ToColor();
        }

        Color outlineColor = default;
        if (isSameLayer)
        {
          if (PaintMode == PaintMode.Pen) outlineColor = Settings.Colors.PaintModeLevelBorder.ToColor();
          else if (PaintMode == PaintMode.Eraser) outlineColor = Settings.Colors.EraserModeLevelBorder.ToColor();
          else if (PaintMode == PaintMode.Inspector) outlineColor = Settings.Colors.InspectorModeLevelBorder.ToColor();
          if (outlineColor != default)
            batcher.DrawRectOutline(camera, layer.Bounds.ExpandFromCenter(new Vector2(7)), outlineColor, 4);
        }

        WorldRenderer.RenderLayer(batcher, camera, layer, color, Settings.Colors.SpriteInstanceName.ToColor());


        if (mouseInLayer 
            && !Selection.HasBegun() 
            && Nez.Input.LeftMouseButtonReleased)
        {
          _imgui.SelectedLevel = index;
        }

        if (Settings.Graphics.DrawLayerGrid 
            && layer is TileLayer tileLayer 
            && isSameLayer)
          Guidelines.GridLines.RenderGridLines(batcher, camera, tileLayer.Bounds.Location, Settings.Colors.LevelGrid.ToColor(), 
              tileLayer.TilesQuantity, tileLayer.TileSize.ToVector2());
      }
    }
    void DrawLevel(int index, Batcher batcher, Camera camera)
    {
      var level = World.Levels[index];
      var hover = level.Bounds.Contains(Camera.MouseToWorldPoint()) && _imgui.SelectedLevel != index && !InputManager.IsImGuiBlocking;
      batcher.DrawRect(level.Bounds, Settings.Colors.LevelSheet.ToColor());

      // Hover level
      if (hover)
      {
        batcher.DrawRectOutline(camera, level.Bounds.ExpandFromCenter(new Vector2(10)), Settings.Colors.SelectionOutline.ToColor(), 4);
      }
      DrawLayers(index, batcher, camera);

      if (_imgui._objHolder != null && _imgui._objHolder.Content == _imgui.TileInstanceInspector)
        batcher.DrawRectOutline(camera, _imgui.SpritePicker._selectedBounds, Settings.Colors.InspectSpriteFill.ToColor());

      if (hover)
      {
        batcher.DrawStringCentered(camera, $"{level.ContentSize.X}x{level.ContentSize.Y}", 
            level.Bounds.TopCenter(), Settings.Colors.WorldSize.ToColor(), new Vector2(2, 40), true, false);
      }
      batcher.DrawString(camera, level.Name, level.Bounds.BottomLeft(), Settings.Colors.WorldName.ToColor(), new Vector2(2, 10));

    }
    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      Guidelines.OriginLinesRenderable.Render(batcher, camera, settings.Colors.OriginLineX.ToColor(), settings.Colors.OriginLineY.ToColor());

      // Selected level outline
      if (_imgui.SelectedLevel != -1 && !Selection.HasBegun())
      {
        batcher.DrawRectOutline(camera, _imgui.SelectedLevelInspector.Level.Bounds.ExpandFromCenter(new Vector2(6)), settings.Colors.LevelSelOutline.ToColor(), 1);
      }

      if (settings.Graphics.FocusOnOneLevel && PaintMode != PaintMode.None && _imgui.SelectedLevel != -1)
      {

        DrawLevel(_imgui.SelectedLevel, batcher, camera);
      }
      else
      {
        for (var i = 0; i < _world.Levels.Count(); i++)
        {
          var level = _world.Levels[i];
          if (!level.IsVisible) continue;
          DrawLevel(i, batcher, camera);
        }
      }

      // if (_imgui.SelectedLevelInspector != null && _imgui.SelectedLevelInspector.CurrentLayer is TileLayer tileLayer)
      
      if (Selection.Capture is Level lev && !Selection.IsEditingPoint)
      {
        lev.LocalOffset = Selection.ContentBounds.Location;
        // Console.WriteLine("Moving at " + lev.Bounds.RenderStringFormat());
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
        instance.Props.Transform.Position = _startScene.Position + (Selection.ContentBounds.Location - Selection.InitialBounds.Location) 
          + instance.Scene.MaxOrigin * scaleDelta;
        instance.Props.Transform.Scale = _startScene.Scale + scaleDelta;
      }
    }
    Vector2 _startLevel = Vector2.Zero;
    Level _startLevelInstance = null;
    Transform _startScene = Transform.Default;
  }
}
