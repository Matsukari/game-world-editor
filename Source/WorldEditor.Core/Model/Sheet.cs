
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Persistence;

namespace Raven
{ 
  /// <summary>
  /// Simply stores the divided sprites into either Tile or SpriteScene from a single texture
  /// </summary>
	public class Sheet : IPropertied
	{
    [JsonInclude]
    public string Name { get; set; } = "";

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    [JsonInclude]
    internal Dictionary<int, Tile> _tiles = new Dictionary<int, Tile>();
    

    /// <summary>
    /// List of all SpriteScenes created in this Sheet
    /// </summary>
		public List<SpriteScene> SpriteScenees = new List<SpriteScene>();

    /// <summary>
    /// List of all Sprite animation created in this Sheet
    /// </summary>
		public List<AnimatedSprite> Animations = new List<AnimatedSprite>();

    /// <summary>
    /// Width of tile in pixels
    /// </summary>
    public int TileWidth;

    /// <summary>
    /// Height of tile in pixels
    /// </summary>
    public int TileHeight; 

    /// <summary>
    /// filename where the image this Sheet uses is loaded
    /// </summary>
    public string Source;

    /// <summary>
    /// The texture this Sheet operates
    /// </summary>
    public Texture2D Texture { get => _texture; }

    /// <summary>
    /// This contains all those Tiles with custom properties or name. Other tiles are merely a rectangle or point 
    /// that describes their source so simply use that rectangle to like a normal sprite sheet to normally render for sprites
    /// </summary>
    public Dictionary<int, Tile> TileMap { get=> _tiles;  }

    /// <summary>
    /// Size of the image in pixels
    /// </summary>
    public Vector2 Size { get => new Vector2(_texture.Width, _texture.Height); }

    /// <summary>
    /// Size of each Tile represented in pixels
    /// </summary>
    public Point TileSize { get => new Point(TileWidth, TileHeight); }

    /// <summary>
    /// Number or rows or columns sliced when set with a TileSize
    /// </summary>
    public Point Tiles { get => new Point(_texture.Width/TileWidth, _texture.Height/TileHeight); }

    internal Texture2D _texture;


    private Sheet()
    {
    }


    /// <summary>
    /// Loads a texture from filename relative to CWD
    /// </summary>
    public Sheet(string filename) 
    {
      Name = Path.Combine(System.Environment.CurrentDirectory, "Untitled.rvsheet");
      var texture = Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(filename));
      texture.Name = filename;
      Source = filename;
      Insist.IsNotNull(texture); 
      _texture = texture;
      SetTileSize(16, 16); 
      
    }
    /// <summary>
    /// Sets the tile size
    /// </summary>
    public void SetTileSize(int w, int h) 
    {
      TileWidth = w;
      TileHeight = h;
    }
    /// <summary>
    /// Finds and returns the SpriteScene with the given name
    /// </summary>
    public SpriteScene GetSpriteScene(string name) => SpriteScenees.Find((spriteScene)=>spriteScene.Name == name);
    /// <summary>
    /// Finds and returns the Tile (with custom properties or name set) in the given Sheet coordinate 
    /// </summary>
    public Tile GetCreatedTile(int coord) => _tiles[coord];

    /// <summary>
    /// Finds and returns the Tile with custom properties or name set in the given Sheet coordinate. 
    /// If no Tile is present in the coordinate, simply create one (but not included in the list) and return it.
    /// </summary>
    public Tile GetTileData(int coord) 
    {
      if (!IsTileValid(coord)) throw new ArgumentOutOfRangeException();
      if (_tiles.ContainsKey(coord)) return _tiles[coord];
      return new Tile(GetTileCoord(coord), this);
    }
    /// <summary>
    /// Creates a source rectangle with tilesize and the given point in Sheet coordinates
    /// </summary>
    public Rectangle GetTile(int x, int y) => new Rectangle(x*TileWidth, y*TileHeight, TileWidth, TileHeight);

    /// <summary>
    /// Creates a source rectangle with tilesize and the given point in Sheet coordinates
    /// </summary>
    public Rectangle GetTile(int index) => GetTile(index % Tiles.X, index / Tiles.X);

    /// <summary>
    /// Gets the coordinates using a source rect from Sheet
    /// </summary>
    public Point GetTile(Rectangle rect) => new Point(rect.X/TileWidth, rect.Y/TileHeight);

    /// <summary>
    /// Converts given Sheet coordinates to an int id which can be used as key in tiles list
    /// </summary>
    public int GetTileId(int x, int y) => y * Tiles.X + x;

    /// <summary>
    /// Converts given World coordinates to an int id which can be used as key in tiles list
    /// </summary>
    public int GetTileIdFromWorld(float x, float y) => GetTileId((int)x/TileWidth, (int)y/TileHeight);

    public Point GetTileCoordFromWorld(float x, float y) => new Point((int)x/TileWidth, (int)y/TileHeight);

    public Point GetTileCoord(int index) => new Point(index % Tiles.X, index / Tiles.X); 

    /// <summary>
    /// Check if the given key id of Tile exist within Sheet
    /// </summary>
    public bool IsTileValid(int index) => index >= 0 && index < Tiles.X * Tiles.Y;

    public void AddScene(SpriteScene scene)
    {
      SpriteScenees.Add(scene);
      SpriteScenees = SpriteScenees.EnsureNoRepeatNameField();
    }
    public void AddAnimation(AnimatedSprite animation)
    {
      Animations.Add(animation);
      Animations = Animations.EnsureNoRepeatNameField();
    }
    public void RemoveScene(SpriteScene scene)
    {
      SpriteScenees.RemoveAll(item => item.Name == scene.Name);
    }


    public void ReplaceScene(string name, SpriteScene scene)
    {
      var index = SpriteScenees.FindIndex(item => item.Name == name);
      if (index != -1) SpriteScenees[index] = scene;
    }
    
    public Tile CustomTileExists(int x, int y) 
    {
      Tile tile = null;
      _tiles.TryGetValue(GetTileId(x, y), out tile);
      return tile;
    }
    public bool CreateTile(Tile tile) 
    { 
      return _tiles.TryAdd(tile.Id, tile);
    }
    public Sprite CreateSprite(string name, params int[] tiles)
    {
      var list = new List<Rectangle>(); 
      foreach (var tile in tiles)
      {
        list.Add(GetTile(tile));
      }
      var newTile = new Sprite(RectangleExt.MinMax(list), this);
      return newTile;
    }
    int _continousCounter = 0;
    public Sprite CreateSprite(params int[] tiles) => CreateSprite($"Sprite_{_continousCounter++}", tiles);
    public SpriteScene CreateSpriteScene(Sprite sprite) => CreateSpriteScene($"SpriteScene_{_continousCounter++}", sprite);
    public SpriteScene CreateSpriteScene(string name, Sprite sprite)
    {
      var main = new SourcedSprite(sprite: sprite);
      var spriteScene = new SpriteScene(name, main, this);
      return spriteScene;
    }
    public List<int> GetTiles(RectangleF container)
    {
      var tiles = new List<int>();
      for (int x = 0; x < Tiles.X; x++)
      {
        for (int y = 0; y < Tiles.Y; y++)
        {
          var tile = GetTile(x, y).ToRectangleF();
          if (container.Intersects(tile) || container.Contains(tile))
          {
            tiles.Add(GetTileId(x, y));
          }
        }
      }
      return tiles;
    }
    public List<Rectangle> GetTiles()
    {
      var list = new List<Rectangle>();
      for (int i = 0; i < Tiles.X * Tiles.Y; i++)
        list.Add(GetTile(i));
      return list;
    }
	}
}
