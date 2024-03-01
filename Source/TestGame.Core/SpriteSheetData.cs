
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
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;
  }
  public struct SpriteKeyFramesData 
  {
    public string Name;
    public float Time;
    private List<RenderState> Frames;
  }
  public struct ComplexSpriteData 
  {  
    public struct PartSpriteData 
    {
      public TiledSpriteData Tile;
      public RenderState LocalState; 
    }
    public string Name;
    public List<Object> Animations;
    public Dictionary<String, PartSpriteData> Parts;
    public Dictionary<String, Object> Properties;    
    public ComplexSpriteData(string name, PartSpriteData main) 
    {
      Name = name;
      Animations = new List<object>();
      Parts = new Dictionary<string, PartSpriteData>();
      Properties = new Dictionary<string, object>();
      Parts.Add("mainn", main);
    }
  }
  public struct TiledSpriteData 
  {  
    public string Name = "";
    public Rectangle Region = new Rectangle();
    public Dictionary<string, Object> Properties = new Dictionary<string, object>();
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
	public class SpriteSheetData
	{
    public string Name = "";
		public Dictionary<String, ComplexSpriteData> Sprites;
    public Dictionary<int, TiledSpriteData> Tiles;
    public Dictionary<String, Object> Properties;
    public int TileWidth;
    public int TileHeight;
    private Texture2D _texture;
    private int _counter = 0;
    public Vector2 Size { get => new Vector2(_texture.Width, _texture.Height); }
    public SpriteSheetData(Texture2D texture) 
    {
      System.Diagnostics.Debug.Assert(texture != null);
      _texture = texture;
      Sprites = new Dictionary<string, ComplexSpriteData>();
      Tiles = new Dictionary<int, TiledSpriteData>();
      Properties = new Dictionary<string, object>();
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
          if (!IntersectTile(tile.Region)) Tiles.Add(GetTile(col, row), tile);
        }
      }
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
        Tiles.Add(tiles.First(), newTile);
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
