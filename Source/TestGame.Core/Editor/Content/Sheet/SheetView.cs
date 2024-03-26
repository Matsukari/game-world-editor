using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Raven.Sheet.Sprites;

namespace Raven.Sheet
{
  public class SheetView : EditorComponent, IImGuiRenderable
  {
    Sheet _sheet;
    SpriteRenderer _image;
    public bool IsDrawGrid = true;
    public Rectangle TileInMouse;
    internal SheetInspector _inspector = new SheetInspector();

    Point lastSize = Point.Zero;
    List<Rectangle> _tiles = new List<Rectangle>();
    TileInspector _tileInspector = new TileInspector();
    SpriteInspector _spriteInspector = new SpriteInspector();

    public override void OnDisabled()
    {
      if (_image != null) _image.Enabled = false;
    }
    public override void OnEnabled()
    {
      if (_image != null) _image.Enabled = true;
    }
    public override void OnContent()
    {
      RenderLayer = -1;
      if (_image != null) Entity.RemoveComponent(_image);
      if (RestrictTo<Sheet>())
      {
        _sheet = Content as Sheet;
        _image = Entity.AddComponent(new SpriteRenderer(_sheet.Texture));
        Entity.GetComponent<Utils.Components.CameraMoveComponent>().Enabled = true;
        Entity.GetComponent<Utils.Components.CameraZoomComponent>().Enabled = true;
      }
    }
    public void Render(Editor editor)
    {
      _inspector.Sheet = _sheet;
      _inspector.Render(editor);
      // Evaluate to either one; 
      _tileInspector.Tile = ContentData.Selection as Tile;
      _tileInspector.Render(editor);

      _spriteInspector.Sprite = ContentData.Selection as Sprite;
      _spriteInspector.Render(editor);
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      SyncModifiedTiles();
      DrawArtifacts(batcher, camera);

      // Draw last selected sprite
      RectangleF region = RectangleF.Empty;
      if (ContentData.Selection is Sprite sprite) region = sprite.Region.ToRectangleF();
      else if (ContentData.Selection is Tile tile) region = tile.Region.ToRectangleF();
      if (region != RectangleF.Empty) batcher.DrawRect(GetRegionInSheet(region), Editor.Settings.Colors.SpriteRegionActiveFill);
    }
    void SyncModifiedTiles()
    {
      if (lastSize != _sheet.TileSize)
      {
        lastSize = _sheet.TileSize;
        _tiles = _sheet.GetTiles();
      }
    }
    // Rectangles & highlights
    void DrawArtifacts(Batcher batcher, Camera camera)
    {
      // Draw tiles' grid
      foreach (var tile in _tiles) 
      {
        var worldTile = tile.ToRectangleF();
        worldTile.Location += _image.Bounds.Location;
        if (worldTile.Contains(camera.MouseToWorldPoint())) TileInMouse = tile;
        if (IsDrawGrid)
        {
          batcher.DrawRectOutline(camera, worldTile, Editor.Settings.Colors.SpriteRegionInactiveOutline);
        }
      }
      // Highlight the tile under mouse
      var worldTileInMouse = TileInMouse.ToRectangleF();
      if (worldTileInMouse != null) 
      {
        worldTileInMouse.Location += _image.Bounds.Location;
        batcher.DrawRectOutline(camera, worldTileInMouse, Editor.Settings.Colors.SpriteRegionActiveFill);
      }
    }

    public override void Update()
    {
      base.Update();
    }
    public RectangleF GetRegionInSheet(RectangleF rectangle)
    {
      rectangle.Location += _image.Bounds.Location;
      return rectangle;
    }
    public Vector2 GetOffRegionInSheet(Vector2 vector)
    {
      vector -= _image.Bounds.Location;
      return vector;
    }
  }
}
