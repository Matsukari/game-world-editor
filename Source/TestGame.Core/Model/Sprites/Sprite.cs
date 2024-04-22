using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Persistence;
using Nez;

namespace Raven 
{
  /// <summary>
  /// This is only a handle to a set of Tiles. Mulitple selection of Tiles.
  /// </summary> 
  public class Sprite : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "";

    [JsonInclude]
    public Rectangle Region { get; private set; } = new Rectangle();

    public List<Tile> GetTiles { get => _createdTiles; }
    public Texture2D Texture { get => _sheet.Texture; }
    public Point TileSize { get => _sheet.TileSize; }

    public Vector2 MinUv { get => Region.Location.ToVector2() / _sheet.Size; }
    public Vector2 MaxUv { get => (Region.Location + Region.Size).ToVector2() / _sheet.Size; }

    internal List<int> _tiles = new List<int>();
    internal List<Tile> _createdTiles = new List<Tile>();

    internal Sheet _sheet;
    private Sprite() {}


    public Sprite(Rectangle region, Sheet sheet)
    {
      Region = region;
      _sheet = sheet;
      _tiles = sheet.GetTiles(region.ToRectangleF());
    }
    public Sprite(Sheet sheet)
    {
      Region = new Rectangle();
      _sheet = sheet;
    }
    
    public void Refer(Sprite sprite)
    {
      Properties = sprite.Properties;
      Name = sprite.Name;
      Region = sprite.Region;
      _sheet = sprite._sheet;
      _tiles = _sheet.GetTiles(Region.ToRectangleF());
    }

    public List<Sprite> SubDivide(Point size)
    {
      List<Sprite> list = new List<Sprite>();
      for (int y = Region.Top; y < Region.Bottom; y += size.Y)
      {
        for (int x = Region.Left; x < Region.Right; x += size.X)
        {
          list.Add(new Sprite(new Rectangle(x, y, size.X, size.Y), _sheet));
        }
      }
      return list;
    }
    public List<Tile> GetRectTiles()
    {
      var list = new List<Tile>();
      foreach (var tile in _tiles)
      {
        list.Add(new Tile(_sheet.GetTileCoord(tile), _sheet));
      }
      return list;
    }
    // Forms a new region based on min and max, in this case max, the coord's region
    public void Rectangular(RectangleF rect)
    {
      Console.WriteLine("About " + rect.RenderStringFormat());
      _tiles = _sheet.GetTiles(rect);
      var list = new List<Tile>();
      foreach (var tile in _tiles)
      {
        list.Add(new Tile(_sheet.GetTileCoord(tile), _sheet));
      }
      Region = list.EnclosedBounds();

    }
    public override bool Equals(object obj)
    {
      if (obj is Sprite sprite && sprite.Name == Name) 
        return sprite.Region == Region;
      return false;
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

      
      
  }
}
