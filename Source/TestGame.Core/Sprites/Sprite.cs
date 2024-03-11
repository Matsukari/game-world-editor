
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
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.BorderNone;
    }

    protected override void OnChangeProperty(string name)
    {
      if (_sheet.CreateTile(this))
        Console.WriteLine("Created ");
    }
    protected override void OnRenderBeforeName()
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Id", Id.ToString());
      ImGui.LabelText("Tile", $"{Coordinates.X}x, {Coordinates.Y}y");
      ImGui.EndDisabled();
    }
      
  }
  public class Sprite : Propertied
  {
    public Rectangle Region { get; private set; } = new Rectangle();
    List<int> _tiles = new List<int>();
    List<Tile> _createdTiles = new List<Tile>();
    public List<Tile> GetTiles { get => _createdTiles; }

    Sheet _sheet;
    public Sprite(Rectangle region, Sheet sheet)
    {
      Region = region;
      _sheet = sheet;
      _tiles = sheet.GetTiles(region.ToRectangleF());
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.GripHorizontal;
    } 
    protected override void OnChangeProperty(string name)
    {
      foreach (var tile in _tiles)
      {
        var tileCoord = _sheet.GetTileCoord(tile);
        // If not created yet
        _sheet.CreateTile(new Tile(_sheet.GetTileCoord(tile), _sheet));
        // Update
        var instanced = _sheet.GetCreatedTile(tile);
        instanced.Properties.OverrideOrAddAll(Properties);
      }
    }
    protected override void OnChangeName(string prev, string curr)
    {
      foreach (var tile in _tiles)
      {
        // If not created yet
        _sheet.CreateTile(new Tile(_sheet.GetTileCoord(tile), _sheet));
        var instanced = _sheet.GetCreatedTile(tile);
        instanced.Name = curr;
      }
    } 
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
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
      public override void RenderImGui(PropertiesRenderer renderer)
      {
        ImGui.BeginDisabled();
        if (SourceSprite.HasName()) ImGui.LabelText("Source", SourceSprite.Name);
        ImGui.LabelText("Tiles", SourceSprite.GetTiles.Count.ToString());
        ImGui.LabelText("Region", SourceSprite.Region.RenderStringFormat());
        ImGui.EndDisabled();
        
        if (ImGui.SmallButton(IconFonts.FontAwesome5.Edit))
        {
          
        }
        ImGui.SameLine();
        ImGui.TextDisabled("Change source");

        Transform.RenderImGui();
        ImGui.LabelText("Origin", Origin.ToString());
      }
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
    public Transform Transform = new Transform();
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
      set 
      {

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
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      var bounds = Bounds.ToNumerics();
      if (ImGui.InputFloat4("Bounds", ref bounds)) Bounds = bounds.ToRectangleF();
      Transform.RenderImGui();
      if (ImGui.CollapsingHeader("Animations", ImGuiTreeNodeFlags.DefaultOpen))
      {

      }
      if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding))
      {
        foreach (var part in Body)
        {
          if (ImGui.BeginChild(part.Name))
          {
            part.RenderImGui(renderer);
            ImGui.NewLine();
            ImGui.EndChild();
          }
        }
      }
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.User;
    }
    protected override void OnChangeName(string old, string now)
    {
      _sheet.Spritexes.ChangeKey(old, now);
    } 
  }
}
