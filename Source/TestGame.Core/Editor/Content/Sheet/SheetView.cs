using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace Raven
{
  public class SelectionList 
  {
    public List<object> Selections = new List<object>();

    // public Enumerable
    public void Add(object sel)
    {
      if (!Nez.InputUtils.IsShiftDown())
        Selections.Clear();
      Selections.Add(sel);
    }
    public void Last() => Selections.Last();
  }
  public class EditorInterface
  {
    public Camera Camera { get; private set; }
    public Selection Selection { get; private set; }
    public IPropertied Content { get; private set; }
    public Serializer Serializer { get; private set; }
    public EditorContentData ContentData { get; private set; }
    public EditorSettings Settings { get; private set; }
    public Entity Entity { get; private set; }

    public virtual void Initialize(Editor editor)
    {
      Entity = editor;
      Selection = editor.Selection;
      Camera = editor.Scene.Camera;
      ContentData = editor.ContentManager.ContentData;
      Content = editor.ContentManager.Content;
      Serializer = editor.Serializer;
      Settings = editor.Settings;
    }
  }
  public abstract class ContentView : EditorInterface
  {
    public virtual IInputHandler InputHandler { get => null; }
    public virtual IImGuiRenderable ImGuiHandler { get => null; }

    public abstract bool CanDealWithType(object content);

    public virtual void OnInitialize(EditorSettings settings) {}
    public virtual void OnContentOpen(IPropertied content) {}
    public virtual void OnContentClose() {}

    public virtual void Render(Batcher batcher, Camera camera, EditorSettings settings) {}
  }
  public class SheetViewImGui : IImGuiRenderable
  {
    readonly EditorSettings _settings;
    public readonly SheetInspector Inspector;
    public SpriteSceneView SceneView;
    TileInspector _tileInspector = new TileInspector();
    SpriteInspector _spriteInspector = new SpriteInspector();

    SelectionList _list;
    Sheet _sheet;

    public SheetViewImGui(EditorSettings settings, Camera camera)
    {
      _settings = settings;
      Inspector = new SheetInspector(settings, camera);
    }
    public void Update(Sheet sheet, SelectionList list) 
    {
      _list = list;
      _sheet = sheet;
    }
    void IImGuiRenderable.Render(ImGuiWinManager imgui)
    {
      if (SceneView != null && SceneView.IsEditing)
      {
        SceneView.SceneInspector.Render(imgui);
        return;
      }
      Inspector.Sheet = _sheet;
      Inspector.Render(imgui);

      if (_list != null && _list.Selections.Count() > 0)
      {
        // Evaluate to either one; 
        _tileInspector.Tile = _list.Selections.Last() as Tile;
        _tileInspector.Render(imgui);

        _spriteInspector.Sprite = _list.Selections.Last() as Sprite;
        _spriteInspector.Render(imgui);
      }

    }
  }

  public class SheetView : ContentView
  {
    public override IImGuiRenderable ImGuiHandler => _imgui;
    public override IInputHandler InputHandler => _input;
    public Sheet Sheet { get => _sheet; }
 
    SheetViewImGui _imgui;
    SheetViewInputHandler _input;

    Sheet _sheet;
    SpriteRenderer _image;
    List<Rectangle> _tiles;
    SpriteSceneView _scene;
    string lastSheet = "";

    public bool IsDrawGrid = true;
    public Rectangle TileInMouse;

    public SheetInspector Inspector { get => _imgui.Inspector; }

    public override bool CanDealWithType(object content) => content is Sheet;

    public override void Initialize(Editor editor)
    {
      _input = new SheetViewInputHandler(this);
      _scene = new SpriteSceneView(this);
      _input.Initialize(editor);
      _scene.Initialize(editor);
      base.Initialize(editor);
    }
    public override void OnInitialize(EditorSettings settings)
    {
      _imgui = new SheetViewImGui(settings, Camera);

      Inspector.OnClickScene += scene => _scene.Edit(scene);
      Inspector.OnDeleteScene += scene => _scene.UnEdit();

      _scene.OnEdit += () => Inspector.ShowPicker = true;
      _scene.OnUnEdit += () => Inspector.ShowPicker = false;

      _imgui.SceneView = _scene;
    }
    public override void OnContentOpen(IPropertied content)
    {
      TileInMouse = Rectangle.Empty;
      _sheet = content as Sheet;
      _image = new SpriteRenderer(_sheet.Texture);
      _image.Entity = Entity;
    }
    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      SyncModifiedTiles();
      _imgui.Update(_sheet, ContentData.SelectionList);

      if (_scene.IsEditing) 
      {
        _scene.Render(batcher, camera);
        return;
      }

      _image.Render(batcher, camera);

      // Draw tiles grid
      foreach (var tile in _tiles) 
      {
        var worldTile = tile.ToRectangleF();
        worldTile.Location += _image.Bounds.Location;
        if (worldTile.Contains(camera.MouseToWorldPoint()) ) TileInMouse = tile;
        if (IsDrawGrid)
        {
          batcher.DrawRectOutline(camera, worldTile, settings.Colors.PickHoverOutline.ToColor());
        }
      }
      // Highlight the tile under mouse
      if (!TileInMouse.IsEmpty && !settings.IsEditorBusy) 
      {
        var worldTileInMouse = TileInMouse.ToRectangleF();
        worldTileInMouse.Location += _image.Bounds.Location;
        batcher.DrawRect(worldTileInMouse, settings.Colors.PickHover.ToColor());
        batcher.DrawRectOutline(camera, worldTileInMouse, settings.Colors.PickSelectedOutline.ToColor());
      }

      // Draw last selected sprite
      foreach (var selection in ContentData.SelectionList.Selections)
      {
        RectangleF region = RectangleF.Empty;
        if (selection is Sprite sprite) region = sprite.Region.ToRectangleF();
        else if (selection is Tile tile) region = tile.Region.ToRectangleF();

        if (region != RectangleF.Empty) 
          batcher.DrawRect(GetRegionInSheet(region), settings.Colors.PickFill.ToColor());

      }
    }
    void SyncModifiedTiles()
    {
      if (lastSheet != _sheet.Name || _tiles == null)
      {
        lastSheet = _sheet.Name;
        _tiles = _sheet.GetTiles();
      }
    }
    public RectangleF GetRegionInSheet(RectangleF rectangle)
    {
      rectangle.Location += _image.Bounds.Location;
      return rectangle;
    }
    public Vector2 GetOffRegionInSheet(Vector2 pos)
    {
      pos -= _image.Bounds.Location;
      return pos;
    }
  }
}
