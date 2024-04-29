using Microsoft.Xna.Framework;
using Nez;

namespace Raven
{
  /// <summary>
  /// Like a pathfinding, from a point, find all position in layer that has no painted tile, 
  /// and assume that the each node path (point) to be travelled is as large as the give size.
  /// This is called in the game loop, just restart to start
  /// </summary>
  public class TileFillFlooder
  {
    TileLayer _layer;
    List<Point> _fill = new List<Point>();
    Queue<Point> _frontier = new Queue<Point>();
    Dictionary<Point, bool> _visited = new Dictionary<Point, bool>();
    static readonly (int, int)[] _dirs = new (int, int)[]{(1, 0), (-1, 0), (0, 1), (0, -1)};
    Action<List<Point>> _callback;
    bool _isCleared = true;
    Tile _flood;

    /// <summary>
    /// Number of times Update will fill a tile
    /// </summary>
    public int MaxIterations = 200;

    public void Start(Action<List<Point>> callback) 
    {
      Clear(); 
      _callback = callback;
    }
    public bool IsFloodComplete()  => _frontier.Count() == 0 && _fill.Count() != 0; 

    public List<Point> Update(TileLayer layer, Point point, Point size)
    {
      _layer = layer;
      if (!_frontier.Contains(point) && _isCleared) 
      {
        _frontier.Enqueue(point);
        if (layer.Tiles.ContainsKey(point)) _flood = layer.Tiles[point].Tile;
        else _flood = null;
        _isCleared = false;
      } 

      var quota = MaxIterations;
      while (_frontier.Count > 0 && quota > 0) 
      {
        var current = _frontier.Dequeue();

        _fill.AddIfNotPresent(current);

        foreach (var dir in _dirs) 
        {
          var next = current;
          next.X += dir.Item1;
          next.Y += dir.Item2;
          if (_layer.IsTileValid(next.X, next.Y) 
              && (!_layer.Tiles.ContainsKey(next) || (_flood != null && _layer.Tiles[next].Tile.Id == _flood.Id))
              && !_visited.ContainsKey(next))
          {
            _frontier.Enqueue(next);
            _visited[next] = true;
          }
        } 
        quota--;
      }
      if (_callback != null && IsFloodComplete())
      {
        _callback.Invoke(_fill);
        Console.WriteLine("Incoke");
        Clear();
      }
      return _fill;
    }
    public void Clear()
    {
      _callback = null;
      _fill.Clear();
      _frontier.Clear();
      _visited.Clear();
      _isCleared = true;
    }
  }

}
