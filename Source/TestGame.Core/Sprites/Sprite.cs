
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

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
    protected override void OnChangeProperty(string name)
    {
      if (_sheet.CreateTile(this))
        Console.WriteLine("Created ");
    }
    protected override void OnRenderBeforeName()
    {
      ImGui.BeginDisabled();
      ImGui.LabelText(Id.ToString(), "Id");
      ImGui.LabelText($"{Coordinates.X} {Coordinates.Y}", "Tile");
      ImGui.EndDisabled();
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
      foreach (var tile in _tiles)
      {
        // If not created yet
        _sheet.CreateTile(new Tile(_sheet.GetTileCoord(tile), _sheet));
        // Update
        var instanced = _sheet.GetCreatedTile(tile);
        instanced.Properties = Properties.Copy();
      }
    }
    protected override void OnRenderAfterName()
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Tiles", _tiles.Count.ToString());
      ImGui.LabelText("Region", Region.RenderStringFormat());
      ImGui.EndDisabled();
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
    Sheet _sheet;
    public SpriteMap Parts = new SpriteMap();
    public Spritex.Sprite MainPart = new Spritex.Sprite();
    public Spritex(string name, Spritex.Sprite main, Sheet sheet) 
    {
      _sheet = sheet;
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
    protected override void OnChangeName(string old, string now)
    {
      _sheet.Spritexes.ChangeKey(old, now);
    } 
  }
}
