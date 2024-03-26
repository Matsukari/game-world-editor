using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Raven.Sheet.Sprites 
{
  public class Sprite : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "";
    public Rectangle Region { get; private set; } = new Rectangle();
    public List<Tile> GetTiles { get => _createdTiles; }
    public Texture2D Texture { get => _sheet.Texture; }
    public Point TileSize { get => _sheet.TileSize; }
    internal List<int> _tiles = new List<int>();
    internal List<Tile> _createdTiles = new List<Tile>();

    internal Sheet _sheet;
    public Sprite(Rectangle region, Sheet sheet)
    {
      Region = region;
      _sheet = sheet;
      _tiles = sheet.GetTiles(region.ToRectangleF());
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
    public void Rectangular(int coord)
    {
      if (!_sheet.IsTileValid(coord)) throw new Exception();
      var rect2 = _sheet.GetTile(coord);
      var region = Region;
      region.Width = rect2.Right - region.X;
      region.Height = rect2.Bottom - region.Y;
      Region = region.ToRectangleF().AlwaysPositive();
      Console.WriteLine("Size: " + Region.Size);
      _tiles = _sheet.GetTiles(Region.ToRectangleF());
    }
  }
}