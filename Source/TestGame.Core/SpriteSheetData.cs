
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Tools
{
	/// <summary>
	/// temporary class used when loading a SpriteAtlas and by the sprite atlas editor
	/// </summary>
  public struct SpriteFramesData 
  {
    public string Name;
    private List<int> KeyFrames;
    public float Fps;
  }
  public struct RenderState
  {
    public Vector2 Position = new Vector2();
    public Vector2 Scale = new Vector2(1f, 1f);
    public float Rotation = 0f;
    public Vector2 Skew = new Vector2();
    public RenderState() {}
    public void Apply(Transform transform)
    {
      transform.LocalPosition = Position;
      transform.LocalScale = Scale;
      transform.LocalRotationDegrees = Rotation;
    }
  }
  public struct SpriteKeyFramesData 
  {
    public string Name;
    public float Time;
    private List<RenderState> Frames;
  }
  public class CustomProperties
  {
    Dictionary<string, object> _props = new Dictionary<string, object>();
    int _counter = 0;
    public void Add<T>(T obj)
    {
      string name = $"{obj.GetType().Name}.{_counter++}";
      if (obj is ProppedObject prop) 
      {
        if (prop.Name != "") name = prop.Name;
      }
      _props.TryAdd(name, obj);
    }
    public void Remove(string name) => _props.Remove(name);
    public Dictionary<string, object>.Enumerator GetEnumerator() { return _props.GetEnumerator(); }
  }
  public interface ProppedObject 
  {
    CustomProperties Properties { get; }
    string Name { get; set; }
  }
  public class ComplexSpriteData : ProppedObject
  {  
    public class PartSpriteData 
    {
      public TiledSpriteData Tile;
      public RenderState LocalState; 
      public Vector2 Origin = new Vector2();
      public PartSpriteData() {}
    }
    public struct SpriteBody
    {
      public Dictionary<string, PartSpriteData> Parts = new Dictionary<string, PartSpriteData>();
      public void Add(string name, PartSpriteData part) {}
      public SpriteBody() {}
    }
    public List<Object> Animations = new List<object>();
    public SpriteBody Body = new SpriteBody();
    public PartSpriteData BodyOrigin = new PartSpriteData();
    public string Name { get; set; } = "";
    public CustomProperties Properties { get; set; } = new CustomProperties();
    public ComplexSpriteData(string name, PartSpriteData main) 
    {
      Name = name;
      BodyOrigin = main;
      // Body.Add("main", BodyOrigin);
    }
    public RectangleF Bounds 
    {
      get 
      {
        var min = BodyOrigin.LocalState.Position;
        var max = BodyOrigin.LocalState.Position + BodyOrigin.Tile.Region.Size.ToVector2();
        var parts = new List<PartSpriteData>(Body.Parts.Values);
        parts.Add(BodyOrigin);
        foreach (var part in parts)
        {
          min.X = Math.Min(min.X, part.LocalState.Position.X + part.Origin.X);
          min.Y = Math.Min(min.Y, part.LocalState.Position.Y + part.Origin.Y);
          max.X = Math.Max(max.X, part.LocalState.Position.X + part.Tile.Region.Size.ToVector2().X + part.Origin.X);
          max.Y = Math.Max(max.Y, part.LocalState.Position.Y + part.Tile.Region.Size.ToVector2().Y + part.Origin.Y);
        }
        return RectangleF.FromMinMax(min, max);
      }
    }
    public List<PartSpriteData> GetFullBody()
    {
      var parts = new List<PartSpriteData>(Body.Parts.Values);
      parts.Add(BodyOrigin);
      return parts;
    }
  }
  public class TiledSpriteData : ProppedObject
  {  
    public int Id = -1;
    public Rectangle Region = new Rectangle();
    public string Name { get; set; } = "";
    public CustomProperties Properties { get; set; } = new CustomProperties();
    public TiledSpriteData() {}
  }
  public enum CustomPropertyType 
  {
    STRING,
    FILE,
    COLOR,
    INT,
    FLOAT,
    BOOL,
    VECTOR2
  };
	public class SpriteSheetData : ProppedObject
	{
    public string Name { get; set; } = "";
    public CustomProperties Properties { get; set; } = new CustomProperties();
		public Dictionary<String, ComplexSpriteData> Sprites = new Dictionary<string, ComplexSpriteData>();
    public Dictionary<int, TiledSpriteData> Tiles = new Dictionary<int, TiledSpriteData>();
    public Vector2 Size { get => new Vector2(_texture.Width, _texture.Height); }
    public int TileWidth = 0;
    public int TileHeight = 0;
    private int _counter = 0;
    private Texture2D _texture;
    public Texture2D Texture { get => _texture; }
    public SpriteSheetData(Texture2D texture) 
    {
      System.Diagnostics.Debug.Assert(texture != null);
      _texture = texture;
    }
    public void Slice(int w, int h) 
    {
      TileWidth = w;
      TileHeight = h;
      for (int col = 0; col <= _texture.Width; col += w)
      {
        for (int row = 0; row <= _texture.Height; row += h) 
        {
          var tile = new TiledSpriteData();
          tile.Region = new Rectangle(col, row, w, h);
          tile.Id = GetTile(col, row);
          if (!IntersectTile(tile.Region)) Tiles.Add(tile.Id, tile);
        }
      }
    }
    public void Delete(TiledSpriteData tile)
    {
      Tiles.Remove(tile.Id);
    }
    public TiledSpriteData CombineContains(TiledSpriteData select, RectangleF container)
    {
      var tiles = new List<int>();
      var tilesRegion = new List<RectangleF>();
      foreach (var (id, tile) in Tiles)
      {
        if (container.Contains(tile.Region.ToRectangleF())) 
        { 
          tilesRegion.Add(tile.Region.ToRectangleF());
          tiles.Add(id);
        }
      }
      // The tile emcompasses other tiles
      if (tiles.Count != 0)
      {
        foreach (var tile in tiles) Tiles.Remove(tile);
        Rectangle combined = RectangleExt.MinMax(tilesRegion); 
        var newTile = new TiledSpriteData();
        newTile.Region = combined;
        newTile.Id = tiles.First();
        Tiles.Add(newTile.Id, newTile);
        return newTile;
      }
      // The tile's own region get smaller; other encompassed tiles are foreer lost 
      else select.Region = container;
      
      return select;
    }
    public void AddSprite(TiledSpriteData tile, string name="unnamed")
    {
      var main = new ComplexSpriteData.PartSpriteData();
      main.Tile = tile;
      main.LocalState = new RenderState();

      if (name == "unnamed") name = $"unnamed_{_counter++}"; 
      var sprite = new ComplexSpriteData(name, main);
      Sprites.Add(name, sprite);
    }
		public void SaveToFile(string filename)
		{
		}
    public int GetTile(int x, int y) => y * _texture.Width + x;
    bool IntersectTile(Rectangle rect) 
    {
      bool result = false;
      foreach (var (_, tile) in Tiles)
      {
        if (rect.Intersects(tile.Region)) result = true;
      }
      return result;
    }
	}

}
