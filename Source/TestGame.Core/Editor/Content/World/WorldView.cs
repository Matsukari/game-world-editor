
using Nez;

namespace Raven
{
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

    public override void Initialize(Editor editor)
    {
      base.Initialize(editor);

      _input = new WorldViewInputHandler(this);
      _input.OnLeftClickLevel += SelectLevel;
      _input.OnRightClickLevel += OpenLevelOptions;
      _input.Initialize(editor);

      _imgui = new WorldViewImGui(_input.Painter); 
      _imgui.Initialize(editor);
      _imgui.Popups.Initialize(editor);
    }

    public override void OnContentOpen(IPropertied content)
    {
      _worldEntity = new WorldEntity(content as World); 
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
