
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
  public struct SpriteKeyFramesData 
  {
    public string Name;
    public struct KeyFrame 
    {
      public Vector2 Position;
      public Vector2 Scale;
      public float Rotation;
      public float Time;
    }
    private List<KeyFrame> Frames;
  }
  public struct ComplexSpriteData 
  {   
    public string Name;
    public List<Object> Animations;
    public Dictionary<String, Rectangle> Parts; 
    public Dictionary<String, Object> Properties;    
  }
  public struct TiledSpriteData 
  {  
    public string Name;
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
    private Texture2D texture;
    public SpriteSheetData(ref Texture2D texture) 
    {
      this.texture = texture;
      Sprites = new Dictionary<string, ComplexSpriteData>();
      Tiles = new Dictionary<int, TiledSpriteData>();
      Properties = new Dictionary<string, object>();
    }

		public void SaveToFile(string filename)
		{
		}
	}

}
