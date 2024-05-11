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

  
    Point _deltaAccum = Point.Zero;
    public TileLayer(Level level, int w, int h) : base(level) 
    {
      TileWidth = w;
      TileHeight = h;
    }

    public override void OnLevelPushed(RectangleF old)
    {
      _deltaAccum += Level.ContentSize - old.Size.ToPoint();
      var delta = _deltaAccum;
      var tileDelta = delta / TileSize;
      var tiles = Tiles.Keys.ToList();
      if (delta != Point.Zero)
      {
        Console.WriteLine($"Delta: {delta}");
      }
      if (tileDelta != Point.Zero)
      {
        _deltaAccum = Point.Zero;
        Console.WriteLine($"Tile pushed: {tileDelta}");
      }
      tiles.Sort((a, b) => b.CompareInGridSpace(a));
      foreach (var tile in tiles)
      {
        Tiles.ChangeKey(tile, tile + tileDelta);
      }
    }
    public override void OnLevelCutoff(SelectionAxis axis, Vector2 delta)
    {
      if (axis.IsDoubleDirection()) throw new Exception("Cannot handle two directions at once or void");
      if (axis == SelectionAxis.None) return;
      if (axis == SelectionAxis.Left) delta.X *= -1;
      if (axis == SelectionAxis.Top) delta.Y *= -1; 

      Console.WriteLine($"Delta: {delta}"); 

      var tileDelta = delta.ToPoint() / TileSize;
      var tiles = Tiles.Keys.ToList();

      Console.WriteLine($"Tile cuttted: {tileDelta}"); 

      foreach (var tile in tiles)
      {
        if (!IsTileValid(tile + tileDelta))
          Tiles.Remove(tile);
      }

    }
    public void SetTileSize(Point size)
    {
      TileWidth = size.X;
      TileHeight = size.Y;
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

      var coord = new Point((int)(start.X/TileWidth), (int)(start.Y/TileHeight)); 
      if (start.ToPoint() == end.ToPoint() && IsTileValid(coord)) 
      {
        onIntersect.Invoke(coord);
        return;
      }

      // Console.WriteLine("Distance: " + Vector2.Distance(start, end));
      for (int i = 0; i < Vector2.Distance(start, end); i++)
      {
        var x = start.X + Mathf.Cos(angle) * i;
        var y = start.Y + Mathf.Sin(angle) * i; 
        coord = new Point((int)(x / TileWidth), (int)(y / TileHeight)); 
        if (IsTileValid(coord)) 
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

      var coord = new Point((int)(start.X/TileWidth), (int)(start.Y/TileHeight)); 
      if (start.ToPoint() == end.ToPoint() && IsTileValid(coord)) 
      {
        yield return coord;
      }
     
      // Console.WriteLine("Distance: " + Vector2.Distance(start, end));
      for (int i = 0; i < Vector2.Distance(start, end); i++)
      {
        var x = start.X + Mathf.Cos(angle) * i;
        var y = start.Y + Mathf.Sin(angle) * i; 
        coord = new Point((int)(x / TileWidth), (int)(y / TileHeight)); 
        if (IsTileValid(coord)) 
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

    public bool IsTileValid(Point point) => IsTileValid(point.X, point.Y);


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
