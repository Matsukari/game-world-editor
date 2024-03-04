

namespace Tools {

  public class WorldRenderable 
  {
      
  }

  public class WorldSprite
  {

  }
  public class WorldLevelLayer
  {
    public string Name;
    public int TileWidth;
    public int TileHeight; 
    public Dictionary<int, int> Tiles;
    public Dictionary<String, Object> Properties;
  }

  public class WorldLevel 
  {
    public List<WorldLevelLayer> Layers;
    public List<SpriteSheetData> SpriteSheets;
    public Dictionary<String, Object> Properties;
    public int Width; 
    public int Height;
  }
  public class World 
  {
    public List<WorldLevel> Levels;
    public Dictionary<String, Object> Properties;
  }

  public class WorldLevelEditor
  {
  }

}
