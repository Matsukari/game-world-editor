using Microsoft.Xna.Framework;
using Nez.Persistence;
using Nez;

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

    public TileInstance TryGetTile(int x, int y) => TryGetTile(new Point(x, y)) ;

    public TileInstance TryGetTile(Point point) 
    {
      if (Tiles.ContainsKey(point))
        return Tiles[point]; 
      return null;
    }

    public void FindTilesIntersectLine(Vector2 start, Vector2 end, Action<Point> onIntersect) 
    {
      var angle = Mathf.Atan2(end.Y-start.Y, end.X-start.X);
     
      // Console.WriteLine("Distance: " + Vector2.Distance(start, end));
      for (int i = 0; i < Vector2.Distance(start, end); i++)
      {
        var x = start.X + Mathf.Cos(angle) * i;
        var y = start.Y + Mathf.Sin(angle) * i; 
        var coord = new Point((int)(x / TileWidth), (int)(y / TileHeight)); 
        var tile = TryGetTile(coord);
        if (tile != null) 
        {
          onIntersect.Invoke(coord);
          i += (TileSize.X + TileSize.Y) / 2 - 1;
        }
        // Console.WriteLine($"Vertex: {coord.X}, {coord.Y}");
      }
    }
    public IEnumerable<Point> FindTilesIntersectLine(Vector2 start, Vector2 end) 
    {
      var angle = Mathf.Atan2(end.Y-start.Y, end.X-start.X);
     
      // Console.WriteLine("Distance: " + Vector2.Distance(start, end));
      for (int i = 0; i < Vector2.Distance(start, end); i++)
      {
        var x = start.X + Mathf.Cos(angle) * i;
        var y = start.Y + Mathf.Sin(angle) * i; 
        var coord = new Point((int)(x / TileWidth), (int)(y / TileHeight)); 
        var tile = TryGetTile(coord);
        if (tile != null) 
        {
          yield return coord;
          i += (TileSize.X + TileSize.Y) / 2 - 1;
        }
        // Console.WriteLine($"Vertex: {coord.X}, {coord.Y}");
      }
    }


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
      if (tile != null)
        Tiles[coord] = new TileInstance(tile, null);
      else Tiles.Remove(coord);
    }

    public void ReplaceTile(Point coord, TileInstance instance)
    {
      if (!IsTileValid(coord.X, coord.Y)) return;
      if (instance != null)
        Tiles[coord] = instance;
      else Tiles.Remove(coord);
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
