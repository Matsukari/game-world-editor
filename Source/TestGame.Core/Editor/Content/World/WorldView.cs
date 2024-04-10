
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

    World _world;

    // Settings
    public bool IsRandomPaint = false;
    public PaintMode PaintMode = PaintMode.Pen;
    public PaintType PaintType = PaintType.Single;
    Vector2 _initialScale = new Vector2();

    public bool CanPaint { get => _imgui.SpritePicker.SelectedSprite != null; }

    public override bool CanDealWithType(object content) => content is World;

    public override void Initialize(Editor editor, EditorContent content)
    {
      base.Initialize(editor, content);

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

    }

    public override void OnContentOpen(IPropertied content)
    {
      _world = content as World; 
    }  
    void OnLeftClickScene(Layer layer, SpriteSceneInstance instance)
    {
      Selection.Begin(instance.ContentBounds, instance);
      _initialScale = instance.Props.Transform.Scale;
      _imgui.SceneInstanceInspector.Scene = instance;
    }
    void OnLeftClickLevel(Level level, int i)
    {
      Selection.Begin(level.Bounds, level);
      _imgui.SelectedLevel = i;
    }
    void OpenLevelOptions(Level level, int i)
    {
      _imgui.Popups.OpenLevelOptions(level);
    }

    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      Guidelines.OriginLinesRenderable.Render(batcher, camera, settings.Colors.OriginLineX.ToColor(), settings.Colors.OriginLineY.ToColor());

      RenderAnnotations(ContentData.PropertiedContext, batcher, camera, settings);
      for (var i = 0; i < _world.Levels.Count(); i++)
      {
        var level = _world.Levels[i];
        if (!level.IsVisible) continue;

        batcher.DrawRect(level.Bounds, settings.Colors.LevelSheet.ToColor());
        foreach (var layer in level.Layers)
        {
          WorldRenderer.RenderLayer(batcher, camera, layer);
          if (layer is TileLayer tileLayer && layer.IsVisible && Settings.Graphics.DrawLayerGrid)
          {
            Guidelines.GridLines.RenderGridLines(batcher, camera, tileLayer.Bounds.Location, settings.Colors.LevelGrid.ToColor(), 
              tileLayer.TilesQuantity, tileLayer.TileSize.ToVector2());
          }
        }
      }

      // if (_imgui.SelectedLevelInspector != null && _imgui.SelectedLevelInspector.CurrentLayer is TileLayer tileLayer)
      
      if (Selection.Capture is Level lev)
      {
        lev.LocalOffset = Selection.ContentBounds.Location;
        lev.ContentSize = Selection.ContentBounds.Size.ToPoint();
      }
      else if (Selection.Capture is SpriteSceneInstance instance)
      {
        instance.Props.Transform.Position = Selection.ContentBounds.Location;
        instance.Props.Transform.Scale = _initialScale + (Selection.ContentBounds.Size / Selection.InitialBounds.Size) - Vector2.One;
      }
      if (_imgui.SelectedLevel != -1)
      {
        batcher.DrawRectOutline(camera, _imgui.SelectedLevelInspector.Level.Bounds, settings.Colors.LevelSelOutline.ToColor());
      }
    }
  }
}
