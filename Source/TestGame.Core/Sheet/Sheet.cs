
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{ 
  /// <summary>
  /// Tree layers:spritexes, sprites, both which is dynamic, and a virtual 
  /// non-immutable in-there tiles of uniform size across the spritesheet image
  /// </summary>
	public class Sheet : Propertied
	{
    public Texture2D Texture { get => _texture; }
		public Dictionary<String, Sprites.Spritex> Spritexes = new Dictionary<string, Sprites.Spritex>();
		public Dictionary<String, Sprites.Sprite> Sprites = new Dictionary<String, Sprites.Sprite>();
    public Dictionary<int, Sprites.Tile> TileMap { get=> _tiles; }
    // Only instanciated when a tile (primarily a rectangle) is assigned a property or anme
		Dictionary<int, Sprites.Tile> _tiles = new Dictionary<int, Sprites.Tile>();

    public Vector2 Size { get => new Vector2(_texture.Width, _texture.Height); }
    public Point TileSize { get => new Point(TileWidth, TileHeight); }
    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }
    public Point Tiles { get => new Point(_texture.Width/TileWidth, _texture.Height/TileHeight); }

    private Texture2D _texture;
    public Sheet(Texture2D texture) 
    {
      Insist.IsNotNull(texture);
      _texture = texture;
    }
    public void SetTileSize(int w, int h) 
    {
      TileWidth = w;
      TileHeight = h;
    }
    public Sprites.Tile GetCreatedTile(int coord) => _tiles[coord];
    public Rectangle GetTile(int x, int y) => new Rectangle(x*TileWidth, y*TileHeight, TileWidth, TileHeight);
    public Rectangle GetTile(int index) => GetTile(index % Tiles.X, index / Tiles.X);
    public Point GetTile(Rectangle rect) => new Point(rect.X/TileWidth, rect.Y/TileHeight);

    public int GetTileId(int x, int y) => y * Tiles.X + x;
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
      var main = new Sprites.Spritex.Sprite();
      main.SourceSprite = sprite;
      main.Transform = new Sprites.Transform();

      return new Sprites.Spritex(name, main, this);
    }
    public List<string> GetSprites(RectangleF container)
    {
      var tiles = new List<string>();
      foreach (var (id, tile) in Sprites)
      {
        if (container.Contains(tile.Region.ToRectangleF())) 
        { 
          tiles.Add(id);
        }
      }
      return tiles;
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
    bool IntersectSprite(Rectangle rect) 
    {
      bool result = false;
      foreach (var (_, tile) in Sprites)
      {
        if (rect.Intersects(tile.Region)) result = true;
      }
      return result;
    }
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      var name = Name;
      int w = TileWidth, h = TileHeight;
      if (ImGui.InputText("Name", ref name, 10)) Name = name;
      ImGui.LabelText(IconFonts.FontAwesome5.File + " File", _texture.Name);
      if (ImGui.InputInt("TileWidth", ref w)) SetTileSize(w, TileHeight);
      if (ImGui.InputInt("TileHeight", ref h)) SetTileSize(TileWidth, h);
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{Tiles.X} tiles");
      ImGui.LabelText("Height", $"{Tiles.Y} tiles");
      ImGui.EndDisabled();
    }

	}

}
