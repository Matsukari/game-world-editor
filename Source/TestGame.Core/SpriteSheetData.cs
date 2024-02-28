
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using Nez.Sprites;

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
    public string Name;
    public Rectangle Region;
    public Dictionary<string, Object> Properties; 
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
    private int _unnamedSpriteCounter = 0;
    public SpriteSheetData(ref Texture2D texture) 
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
          tile.Properties = new Dictionary<string, object>();
          tile.Name = "";
          tile.Region = new Rectangle(col, row, w, h);
          Tiles.Add(GetTile(col, row), tile);
        }
      }
    }
    public void AddSprite(TiledSpriteData tile, string name="unnamed")
    {
      var main = new ComplexSpriteData.PartSpriteData();
      main.Tile = tile;
      main.LocalState = new RenderState();
      if (name == "unnamed") name = $"unnamed_{_unnamedSpriteCounter}"; 
      var sprite = new ComplexSpriteData(name, main);
      Sprites.Add(name, sprite);
    }
    public int GetTile(int x, int y) => y * _texture.Width + x;
		public void SaveToFile(string filename)
		{
		}
	}

}
