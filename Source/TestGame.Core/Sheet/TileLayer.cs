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
    public List<Point> TileHighlights = new List<Point>();
    public Dictionary<Point, InstancedSprite> Tiles { get => _tiles; }
    Dictionary<Point, InstancedSprite> _tiles = new Dictionary<Point, InstancedSprite>();

    public Point GetTileCoordFromWorld(Vector2 point) => new Point(
        (int)(point.X - Level.Bounds.Location.X) / TileWidth, 
        (int)(point.Y - Level.Bounds.Location.Y) / TileHeight);
    public Point GetTile(int coord) => new Point(coord % TilesQuantity.X, coord / TilesQuantity.X); 
    public bool IsTileValid(int x, int y) => !(x < 0 || x >= TilesQuantity.X || y < 0 || y >= TilesQuantity.Y); 
    public void ReplaceTile(Point point, InstancedSprite tile) => ReplaceTile(point.X, point.Y, tile);
    public void ReplaceTile(int x, int y, InstancedSprite tile)
    {
      if (!IsTileValid(x, y)) return;
      _tiles[new Point(x, y)] = tile;
    }
    public override void Draw(Batcher batcher, Camera camera)
    {
      foreach (var block in _tiles)
      {
        var bounds = new RectangleF(block.Key.X*TileWidth, block.Key.Y*TileHeight, TileWidth, TileHeight);
        bounds.Location += Level.Bounds.Location;
        bounds.Location += Offset;
        switch (block.Value)
        {
          case TileInstance tile: tile.Draw(batcher, camera, bounds); break;
        }
      }
      foreach (var tile in TileHighlights)
      {
        var bounds = new RectangleF(tile.X*TileWidth, tile.Y*TileHeight, TileWidth, TileHeight);
        bounds.Location += Level.Bounds.Location;
        _tiles[tile].Draw(batcher, camera, bounds);
      }
    }

  }
}
