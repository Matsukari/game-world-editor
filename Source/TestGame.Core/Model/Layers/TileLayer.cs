using Microsoft.Xna.Framework;

namespace Raven
{

  /// <summary>
  /// A Layer that can only contains Tiles.
  /// </summary> 
  public class TileLayer : Layer
  {
    public TileLayer(Level level, int w, int h) : base(level) 
    {
      TileWidth = w;
      TileHeight = h;
    }
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
    public Dictionary<Point, Tile> Tiles { get => _tiles; private set => _tiles = value; }

    /// <summary>
    // The list of all custom render peoperties associated to a Tile
    /// </summary> 
    public Dictionary<Point, RenderProperties> TilesProp = new Dictionary<Point, RenderProperties>();

    Dictionary<Point, Tile> _tiles = new Dictionary<Point, Tile>();

    public Point GetTileCoordFromWorld(Vector2 point) => new Point(
        (int)(point.X - Bounds.Location.X) / TileWidth, 
        (int)(point.Y - Bounds.Location.Y) / TileHeight);

    public Point GetTile(int coord) => new Point(coord % TilesQuantity.X, coord / TilesQuantity.X); 

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
      _tiles[coord] = tile;
    }


    /// <summary>
    /// Replaces or create custom render properties (such as Transform, Color) for a specific Tile at the given position
    /// </summary>
    public RenderProperties ReplaceTileProperties(int x, int y)
    {
      if (!IsTileValid(x, y)) throw new Exception("Tile coordinates invalid");
      var loc = new Point(x, y);
      var prop = new RenderProperties();
      TilesProp[loc] = prop;
      return prop;
    }


    /// <summary>
    /// Removed the tile that is contained in the coordinate provided
    /// </summary>
    public void RemoveTile(int x, int y)
    {
      if (!IsTileValid(x, y)) return;
      var loc = new Point(x, y);
      _tiles.Remove(loc);
    }


    public void RemoveTile(Point point) => RemoveTile(point.X, point.Y);

    public override Layer Copy()
    {
      var layer = MemberwiseClone() as TileLayer;
      layer._tiles = new Dictionary<Point, Tile>();
      foreach (var item in _tiles)
      {
        layer._tiles.Add(item.Key, item.Value.Copy());
      }
      layer.TilesProp = new Dictionary<Point, RenderProperties>();
      foreach (var item in TilesProp)
      {
        layer.TilesProp.Add(item.Key, item.Value.Copy());
      }

      return layer;
    }

      

  }
}
