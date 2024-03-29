
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Persistence;

namespace Raven.Sheet
{ 
  /// <summary>
  /// Simply stores the divided sprites into either Tile or Spritex from a single texture
  /// </summary>
	public class Sheet : IPropertied
	{
    [JsonInclude]
    public string Name { get; set; } = "";

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Filename;

    /// <summary>
    /// Only those with custom properties or name will be added in the list
    /// </summary>
    [JsonInclude]
    internal Dictionary<int, Sprites.Tile> _tiles = new Dictionary<int, Sprites.Tile>();
    
		public List<Sprites.Spritex> Spritexes = new List<Sprites.Spritex>();

    public int TileWidth;
    public int TileHeight; 

    public Texture2D Texture { get => _texture; }

    // Only instanciated when a tile (primarily a rectangle) is assigned a property or anme
    public Dictionary<int, Sprites.Tile> TileMap { get=> _tiles;  }

    public Vector2 Size { get => new Vector2(_texture.Width, _texture.Height); }

    public Point TileSize { get => new Point(TileWidth, TileHeight); }

    public Point Tiles { get => new Point(_texture.Width/TileWidth, _texture.Height/TileHeight); }

    internal Texture2D _texture;

    private Sheet()
    {
    }
    public Sheet(string filename) 
    {
      Name = filename;
      Filename = filename;
      var texture = Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(filename));
      texture.Name = filename;
      Insist.IsNotNull(texture); 
      _texture = texture;
      SetTileSize(16, 16); 
    }
    public void SetTileSize(int w, int h) 
    {
      TileWidth = w;
      TileHeight = h;
    }
    public Sprites.Spritex GetSpritex(string name) => Spritexes.Find((spritex)=>spritex.Name == name);
    public Sprites.Tile GetCreatedTile(int coord) => _tiles[coord];
    public Sprites.Tile GetTileData(int coord) 
    {
      if (!IsTileValid(coord)) throw new ArgumentOutOfRangeException();
      if (_tiles.ContainsKey(coord)) return _tiles[coord];
      return new Sprites.Tile(GetTileCoord(coord), this);
    }
    public Rectangle GetTile(int x, int y) => new Rectangle(x*TileWidth, y*TileHeight, TileWidth, TileHeight);
    public Rectangle GetTile(int index) => GetTile(index % Tiles.X, index / Tiles.X);
    public Point GetTile(Rectangle rect) => new Point(rect.X/TileWidth, rect.Y/TileHeight);

    public int GetTileId(int x, int y) => y * Tiles.X + x;
    public int GetTileIdFromWorld(float x, float y) => GetTileId((int)x/TileWidth, (int)y/TileHeight);
    public Point GetTileCoordFromWorld(float x, float y) => new Point((int)x/TileWidth, (int)y/TileHeight);

    public Point GetTileCoord(int index) => new Point(index % Tiles.X, index / Tiles.X); 

    public bool IsTileValid(int index) => index >= 0 && index < Tiles.X * Tiles.Y;

    
    public Sprites.Tile CustomTileExists(int x, int y) 
    {
      Sprites.Tile tile = null;
      _tiles.TryGetValue(GetTileId(x, y), out tile);
      return tile;
    }
    public bool CreateTile(Sprites.Tile tile) 
    { 
      return _tiles.TryAdd(tile.Id, tile);
    }
    public Sprites.Sprite CreateSprite(string name, params int[] tiles)
    {
      var list = new List<Rectangle>(); 
      foreach (var tile in tiles)
      {
        list.Add(GetTile(tile));
      }
      var newTile = new Sprites.Sprite(RectangleExt.MinMax(list), this);
      return newTile;
    }
    int _continousCounter = 0;
    public Sprites.Sprite CreateSprite(params int[] tiles) => CreateSprite($"Sprite_{_continousCounter++}", tiles);
    public Sprites.Spritex CreateSpritex(Sprites.Sprite sprite) => CreateSpritex($"Spritex_{_continousCounter++}", sprite);
    public Sprites.Spritex CreateSpritex(string name, Sprites.Sprite sprite)
    {
      var main = new Sprites.SourcedSprite(sprite: sprite);
      var spritex = new Sprites.Spritex(name, main, this);
      return spritex;
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
