
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Raven.Sheet
{ 
	public class SpriteSheet : IPropertied
	{
    public string Name { get; set; } = "";
    public PropertyList Properties { get; set; } = new PropertyList();
		public Dictionary<String, Sprites.Spritex> Spritexes = new Dictionary<string, Sprites.Spritex>();
    public Dictionary<int, Sprites.Sprite> Sprites = new Dictionary<int, Sprites.Sprite>();
    public Vector2 Size { get => new Vector2(_texture.Width, _texture.Height); }
    public int TileWidth = 0;
    public int TileHeight = 0;
    private int _counter = 0;
    private Texture2D _texture;
    public Texture2D Texture { get => _texture; }
    public SpriteSheet(Texture2D texture) 
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
          var tile = new Sprites.Sprite();
          tile.Region = new Rectangle(col, row, w, h);
          tile.Id = GetTile(col, row);
          if (!IntersectTile(tile.Region)) Sprites.Add(tile.Id, tile);
        }
      }
    }
    public void Delete(Sprites.Sprite sprite)
    {
      Sprites.Remove(sprite.Id);
    }
    public Sprites.Sprite CombineContains(Sprites.Sprite select, RectangleF container)
    {
      var tiles = new List<int>();
      var tilesRegion = new List<RectangleF>();
      foreach (var (id, tile) in Sprites)
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
        foreach (var tile in tiles) Sprites.Remove(tile);
        Rectangle combined = RectangleExt.MinMax(tilesRegion); 
        var newTile = new Sprites.Sprite();
        newTile.Region = combined;
        newTile.Id = tiles.First();
        Sprites.Add(newTile.Id, newTile);
        return newTile;
      }
      // The tile's own region get smaller; other encompassed tiles are foreer lost 
      else select.Region = container;
      
      return select;
    }
    public void AddSprite(Sprites.Sprite tile, string name="unnamed")
    {
      var main = new Sprites.Spritex.Sprite();
      main.SourceSprite= tile;
      main.Transform = new Sprites.Transform();

      if (name == "unnamed") name = $"unnamed_{_counter++}"; 
      var sprite = new Sprites.Spritex(name, main);
      Spritexes.Add(name, sprite);
    }
    public int GetTile(int x, int y) => y * _texture.Width + x;
    bool IntersectTile(Rectangle rect) 
    {
      bool result = false;
      foreach (var (_, tile) in Sprites)
      {
        if (rect.Intersects(tile.Region)) result = true;
      }
      return result;
    }
	}

}
