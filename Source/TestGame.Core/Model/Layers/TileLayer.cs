using Microsoft.Xna.Framework;
using Nez.Persistence;

namespace Raven
{

  /// <summary>
  /// A Layer that can only contains Tiles.
  /// </summary> 
  public class TileLayer : Layer
  {
    /// <summary>
    // The width of an individual Tile
    /// </summary> 
    public int TileWidth;

    /// <summary>
    // The height of an individual Tile
    /// </summary> 
    public int TileHeight;

    public Point TileSize { get => new Point(TileWidth, TileHeight); }

    /// <summary>
    // The number of all Tiles horizontally and vertically
    /// </summary> 
    public Point TilesQuantity { get => new Point(Level.ContentSize.X/TileWidth, Level.ContentSize.Y/TileHeight); }

    /// <summary>
    // The list of all painted Tiles
    /// </summary> 
    [JsonInclude]
    public Dictionary<Point, TileInstance> Tiles { get; private set;} = new Dictionary<Point, TileInstance>();


    internal TileLayer()
    {
    }


    public TileLayer(Level level, int w, int h) : base(level) 
    {
      TileWidth = w;
      TileHeight = h;
    }

    public Point GetTileCoordFromWorld(Vector2 point) => new Point(
        (int)(point.X - Bounds.Location.X) / TileWidth, 
        (int)(point.Y - Bounds.Location.Y) / TileHeight);

    public Point GetTile(int coord) => new Point(coord % TilesQuantity.X, coord / TilesQuantity.X); 

    public TileInstance GetTile(Point point) => Tiles[point]; 

    /// <summary>
    // Checks if the layer contains the given corrdinate 
    /// </summary> 
    public bool IsTileValid(int x, int y) => !(x < 0 || x >= TilesQuantity.X || y < 0 || y >= TilesQuantity.Y); 


    /// <summary>
    /// Replaces tile that is contained in the coordinates provided
    /// </summary>
    public void ReplaceTile(int x, int y, Tile tile) => ReplaceTile(new Point(x, y), tile);

    /// <summary>
    /// Replaces tile that is contained in the coordinates provided
    /// </summary>
    public void ReplaceTile(Point coord, Tile tile)
    {
      if (!IsTileValid(coord.X, coord.Y)) return;
      Tiles[coord] = new TileInstance(tile, null);
    }

    public void ReplaceTile(Point coord, TileInstance instance)
    {
      if (!IsTileValid(coord.X, coord.Y)) return;
      Tiles[coord] = instance;
    }

    /// <summary>
    /// Removed the tile that is contained in the coordinate provided
    /// </summary>
    public void RemoveTile(int x, int y)
    {
      if (!IsTileValid(x, y)) return;
      var loc = new Point(x, y);
      Tiles.Remove(loc);
    }


    public void RemoveTile(Point point) => RemoveTile(point.X, point.Y);

    public override Layer Copy()
    {
      var layer = MemberwiseClone() as TileLayer;
      layer.Tiles = Tiles.CloneItems(); 
      return layer;
    }

      

  }
}
