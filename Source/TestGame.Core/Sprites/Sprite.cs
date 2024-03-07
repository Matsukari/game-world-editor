
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet.Sprites 
{
  public class Tile : Propertied 
  {
    public Point Coordinates;
    public Rectangle Region { get=>_sheet.GetTile(Id); }
    public int Id { get=>_sheet.GetTileId(Coordinates.X, Coordinates.Y); }
    Sheet _sheet;
    public Tile(Point coord, Sheet sheet)
    {
      Coordinates = coord;
      _sheet = sheet;
      Insist.IsTrue(_sheet.IsTileValid(_sheet.GetTileId(coord.X, coord.Y)));
    }
    protected override void OnCreateProperty(string name)
    {
      if (_sheet.CreateTile(this))
        Console.WriteLine("Created ");
    } 
  }
  public class Sprite : Propertied
  {  
    public Rectangle Region { get; private set; } = new Rectangle();
    List<int> _tiles = new List<int>();
    List<Tile> _createdTiles = new List<Tile>();

    Sheet _sheet;
    public Sprite(Rectangle region, Sheet sheet)
    {
      Region = region;
      _sheet = sheet;
      _tiles = sheet.GetTiles(region.ToRectangleF());
    }
    protected override void OnChangeProperty(string name)
    {
      foreach (var tile in _createdTiles)
      {
        foreach (var prop in Properties)
        {
          tile.Properties.Data[prop.Key] = prop.Value;
        }
      }
    } 
    protected override void OnCreateProperty(string name)
    {
      foreach (var tile in _tiles)
      {
        var newTile = new Tile(_sheet.GetTileCoord(tile), _sheet);
        newTile.Name = Name;
        if (_sheet.CreateTile(newTile))
        {
          Console.WriteLine("Created ");
          _createdTiles.Add(newTile);
        }
      }
    } 
  }
  public class Spritex : Propertied
  {  
    public class Sprite : Propertied 
    {
      public Sprites.Sprite SourceSprite;
      public Sprites.Transform Transform; 
      public Vector2 Origin = new Vector2();
      public Sprite() {}
    }
    public struct SpriteMap
    {
      public Dictionary<string, Spritex.Sprite> Data = new Dictionary<string, Spritex.Sprite>();
      public void Add(string name, Spritex.Sprite part) {}
      public SpriteMap() { }
    }
    public SpriteMap Parts = new SpriteMap();
    public Spritex.Sprite MainPart = new Spritex.Sprite();
    public Spritex(string name, Spritex.Sprite main) 
    {
      Name = name;
      MainPart = main;
      MainPart.Name = "Main";
    }
    public RectangleF Bounds 
    {
      get 
      {
        var min = MainPart.Transform.Position;
        var max = MainPart.Transform.Position + MainPart.SourceSprite.Region.Size.ToVector2();
        foreach (var part in Body)
        {
          min.X = Math.Min(min.X, part.Transform.Position.X + part.Origin.X);
          min.Y = Math.Min(min.Y, part.Transform.Position.Y + part.Origin.Y);
          max.X = Math.Max(max.X, part.Transform.Position.X + part.SourceSprite.Region.Size.ToVector2().X + part.Origin.X);
          max.Y = Math.Max(max.Y, part.Transform.Position.Y + part.SourceSprite.Region.Size.ToVector2().Y + part.Origin.Y);
        }
        return RectangleF.FromMinMax(min, max);
      }
    }
    public List<Spritex.Sprite> Body
    {
      get
      {
        var parts = new List<Spritex.Sprite>(Parts.Data.Values);
        parts.Add(MainPart);
        return parts;
      }
    }
  }
}
