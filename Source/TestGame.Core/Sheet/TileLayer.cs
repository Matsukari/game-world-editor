using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  // <summary>
  // Any tile size. Can accept tiles of arbitrary sizes; scales them to fit this layer's tile size
  // </summary> 
  public class TileLayer : Layer
  {
    public TileLayer(Level level, int w, int h) : base(level) 
    {
      TileWidth = w;
      TileHeight = h;
    }
    public int TileWidth;
    public int TileHeight;
    public Point TileSize { get => new Point(TileWidth, TileHeight); }
    public Point TilesQuantity { get => new Point(Level.ContentSize.X/TileWidth, Level.ContentSize.Y/TileHeight); }
    public List<Tile> TileHighlights = new List<Tile>();
    public Dictionary<Point, InstancedSprite> Tiles { get => _tiles; }
    Dictionary<Point, InstancedSprite> _tiles = new Dictionary<Point, InstancedSprite>();

    public Point GetTileCoordFromWorld(Vector2 point) => new Point(
        (int)(point.X - Bounds.Location.X) / TileWidth, 
        (int)(point.Y - Bounds.Location.Y) / TileHeight);
    public Point GetTile(int coord) => new Point(coord % TilesQuantity.X, coord / TilesQuantity.X); 
    public bool IsTileValid(int x, int y) => !(x < 0 || x >= TilesQuantity.X || y < 0 || y >= TilesQuantity.Y); 
    public void ReplaceTile(Point point, InstancedSprite tile) => ReplaceTile(point.X, point.Y, tile);
    public void ReplaceTile(int x, int y, InstancedSprite tile)
    {
      if (!IsTileValid(x, y)) return;
      var loc = new Point(x, y);
      _tiles[loc] = tile;
    }
    public override void Draw(Batcher batcher, Camera camera)
    {
      foreach (var block in _tiles)
      {
        var bounds = new RectangleF(block.Key.X*TileWidth, block.Key.Y*TileHeight, TileWidth, TileHeight);
        bounds.Location += Bounds.Location;
        switch (block.Value)
        {
          case TileInstance tile: tile.Draw(batcher, camera, bounds); break;
          case SpritexInstance spritex: spritex.Draw(batcher, camera, bounds); break;
        }
      }
      foreach (var tile in TileHighlights)
      {
        var bounds = new RectangleF(tile.Region.X, tile.Region.Y, TileWidth, TileHeight);
        bounds.Location += Bounds.Location;
        var t = new TileInstance(tile);
        t.Draw(batcher, camera, bounds);
      }
    }

  }
}
