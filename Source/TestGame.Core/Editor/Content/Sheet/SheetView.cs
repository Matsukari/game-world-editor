using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace Raven
{
  public abstract class ContentView
  {
    public virtual IInputHandler InputHandler { get; }
    public virtual IImGuiRenderable ImGuiHandler { get; }

    public abstract bool CanDealWithType(object content);

    public virtual void OnContentOpen(IPropertied content) {}
    public virtual void OnContentClose() {}

    public virtual void Render(Batcher batcher, Camera camera, EditorSettings settings) {}
  }
  public class SheetView : ContentView
  {
    Sheet _sheet;
    SpriteRenderer _image;
    public bool IsDrawGrid = true;
    public Rectangle TileInMouse;
    internal SheetInspector _inspector = new SheetInspector();

    string lastSheet = "";
    List<Rectangle> _tiles = new List<Rectangle>();
    TileInspector _tileInspector = new TileInspector();
    SpriteInspector _spriteInspector = new SpriteInspector();

    public List<object> Selections = new List<object>();

    public override bool CanDealWithType(object content) => content is Sheet;

    public override void OnContentOpen(IPropertied content)
    {
      TileInMouse = Rectangle.Empty;
      _sheet = content as Sheet;
      _image = new SpriteRenderer(_sheet.Texture);
    }
    public void Render(Editor editor)
    {
      _inspector.Sheet = _sheet;
      _inspector.Render(editor);
      // Evaluate to either one; 
      _tileInspector.Tile = Selections.Last() as Tile;
      _tileInspector.Render(editor);

      _spriteInspector.Sprite = Selections.Last() as Sprite;
      _spriteInspector.Render(editor);
    }
    public override void Render(Batcher batcher, Camera camera, EditorSettings settings)
    {
      SyncModifiedTiles();

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
      foreach (var selection in Selections)
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
      if (lastSheet != _sheet.Name)
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
  }
}
