
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using ImGuiNET;

namespace Raven.Sheet.Sprites 
{
  public class InstancedSprite : Propertied
  {
    public Transform Transform = new Transform();
    public Color Color = Color.White;
    public SpriteEffects SpriteEffects = SpriteEffects.None;
    public virtual void Draw(Batcher batcher, Camera camera, RectangleF dest) {}
  }
  public class TileInstance : InstancedSprite
  {
    Tile _tile;
    public TileInstance(Tile tile) => _tile = tile;
    public override void Draw(Batcher batcher, Camera camera, RectangleF dest)
    {
      var scale = Transform.Scale;
      scale.X *= dest.Width / _tile.Region.Width;
      scale.Y *= dest.Height / _tile.Region.Height;
      batcher.Draw(
        texture: _tile.Texture,
        position: dest.Location + Transform.Position,
        sourceRectangle: _tile.Region,
        color: Color,
        rotation: Transform.Rotation,
        origin: new Vector2(),
        scale: scale,
        effects: SpriteEffects,
        layerDepth: 0);
    }
  }
  public class SpritexInstance : InstancedSprite
  {

  }
  public class Tile : Propertied 
  { 
    public Point Coordinates;
    public Rectangle Region { get=>_sheet.GetTile(Id); }
    public int Id { get=>_sheet.GetTileId(Coordinates.X, Coordinates.Y); }
    public Texture2D Texture { get => _sheet.Texture; }
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
    protected override void OnChangeName(string prev, string curr)
    {
      if (_sheet.CreateTile(this))
        Console.WriteLine("Created tile");
      _sheet.GetCreatedTile(Id).Name = curr;
    }        
    protected override void OnChangeProperty(string name)
    {
      if (_sheet.CreateTile(this))
        Console.WriteLine("Created tile");
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
    public List<Tile> GetTiles { get => _createdTiles; }
    public Texture2D Texture { get => _sheet.Texture; }
    public Point TileSize { get => _sheet.TileSize; }
    List<int> _tiles = new List<int>();
    List<Tile> _createdTiles = new List<Tile>();

    Sheet _sheet;
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
    public void Rectangular(int coord)
    {
      if (!_sheet.IsTileValid(coord)) throw new Exception();
      var list = new List<Rectangle>();
      var rect2 = _sheet.GetTile(coord);
      list.AddIfNotPresent(Region);
      list.AddIfNotPresent(_sheet.GetTile(coord));
      var region = Region;
      region.Width = rect2.Right - region.X;
      region.Height = rect2.Bottom - region.Y;
      Region = region.ToRectangleF().AlwaysPositive();
      _tiles = _sheet.GetTiles(Region.ToRectangleF());
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.GripHorizontal;
    } 
    protected override void OnChangeProperty(string name)
    {
      foreach (var tile in _tiles)
      {
         var existingTile = _sheet.GetTileData(tile);
        // If not created yet
        _sheet.CreateTile(existingTile);
        existingTile.Properties.OverrideOrAddAll(Properties);
      }
    }
    protected override void OnChangeName(string prev, string curr)
    {
      foreach (var tile in _tiles)
      {
        // If not created yet
        var existingTile = _sheet.GetTileData(tile);
        _sheet.CreateTile(existingTile);
        existingTile.Name = curr;
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
  public class Spritex : RenderableComponent
  {
    internal Sheet _sheet;
    public string Name;
    public SpriteMap Parts = new SpriteMap();
    public List<SourcedSprite> Body { get => new List<SourcedSprite>(Parts.Data.Values); } 
    public Spritex(string name, SourcedSprite main, Sheet sheet) 
    {
      Name = name;
      Parts.Add(name, main);
      main.Spritex = this;
      _sheet = sheet;
    }
    public RectangleF EnclosingBounds
    {
      get 
      {
        var min = new Vector2(100000, 100000);
        var max = new Vector2(-10000, -10000);
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
    public override RectangleF Bounds
    {
      get 
      {
				if (_areBoundsDirty)
				{
          _bounds = EnclosingBounds;
          _bounds.CalculateBounds(Transform.Position, _localOffset, _bounds.Size/2f, Transform.Scale, Transform.Rotation, _bounds.Width, _bounds.Height);
          _areBoundsDirty = true;
        }
        return _bounds;
      }
    }
    public SourcedSprite AddSprite(string name, SourcedSprite sprite=null) 
    {
      if (sprite == null)
      {
        sprite = new SourcedSprite();
      }
      sprite.Spritex = this;
      Parts.Add(name, sprite);
      return sprite;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      foreach (var sprite in Body)
      {
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: Transform.Position + LocalOffset + sprite.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: sprite.Color,
            rotation: Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: _layerDepth);
      }
    }
    public class SpriteMap
    {
      public Dictionary<string, SourcedSprite> Data = new Dictionary<string, SourcedSprite>();
      public void Add(string name, SourcedSprite part) { Data.TryAdd(name, part); part.Name = name; }
      public SpriteMap() { }
    }

  }
  public class SourcedSprite
  {
    public string Name = "";
    public Spritex Spritex;
    public Sprite SourceSprite;
    public Sprites.Transform Transform = new Transform(); 
    public SpriteEffects SpriteEffects;
    public Vector2 Origin = new Vector2();
    public Color Color = Color.White;

    // Local bounds
    public RectangleF LocalBounds { 
      get => new RectangleF(
          Transform.Position.X - Origin.X * Transform.Scale.X, 
          Transform.Position.Y - Origin.Y * Transform.Scale.Y, 
          SourceSprite.Region.Width * Transform.Scale.X, 
          SourceSprite.Region.Height * Transform.Scale.Y);
    }
    public RectangleF WorldBounds { 
      get 
      {
        var bounds = LocalBounds;
        bounds.Location += Spritex.Transform.Position;
        bounds.Size *= Spritex.Transform.Scale;
        return bounds;
      }
    }
    public SourcedSprite(Spritex spritex=null, Sprites.Sprite sprite=null) 
    {
      Spritex = spritex;
      SourceSprite = sprite;
    }
  }

  public class SpritexGui : Propertied
  {  
    public Spritex Spritex;
    public Vector2 GuiPosition = new Vector2();
    public float GuiZoom = 0.5f;

    public SpritexGui(Spritex spritex) 
    {
      Spritex = spritex;
    }
    int _originType = 1;
    void Sprite_RenderImGui(SourcedSprite sprite, PropertiesRenderer renderer)
    {
      ImGui.BeginDisabled();
      if (sprite.SourceSprite.HasName()) 
        ImGui.LabelText("Source", sprite.SourceSprite.Name);
      ImGui.LabelText("Region", sprite.SourceSprite.Region.RenderStringFormat());
      ImGui.EndDisabled();

      sprite.Transform.RenderImGui();
      var origin = sprite.Origin.ToNumerics();
     
      if (ImGui.Combo("Origin", ref _originType, new string[]{"Center", "Topleft", "Custom"}, 3))
      {
        if (_originType == 0) sprite.Origin = sprite.LocalBounds.Size/2f;
        else if (_originType == 1) sprite.Origin = new Vector2();
      }
      if (_originType == 2)
      {
        if (ImGui.InputFloat2("Origin", ref origin)) sprite.Origin = origin;
      }
    }

    public SourcedSprite ChangePart = null;
    SourcedSprite  _spritexPart;
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      Transform.RenderImGui(Spritex.Transform);
      if (ImGui.CollapsingHeader("Animations", ImGuiTreeNodeFlags.DefaultOpen))
      {
      }
      if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding))
      {
        foreach (var part in Spritex.Body)
        {
          if (ImGui.TreeNodeEx(part.Name, ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.DefaultOpen))
          {
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
              ImGui.TreePop();
              ImGui.OpenPopup("sprite-component-options");
              _spritexPart = part;
              return;
            }
            ImGui.PushID("spritex-component-name" + part.Name);
            string name = part.Name;
            if (ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
            {
              Spritex.Parts.Data.ChangeKey(part.Name, name);
              part.Name = name;
            }
            ImGui.PopID();
            Sprite_RenderImGui(part, renderer);

            // Options
            ImGui.TreePop();
          }
          if (part.WorldBounds.Contains(renderer.Scene.Camera.MouseToWorldPoint()) && Nez.Input.RightMouseButtonPressed)
          {
            ImGui.OpenPopup("sprite-component-options");
            _spritexPart = part;
          }
        }
        if (ImGui.BeginPopupContextItem("sprite-component-options"))
        {
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Trash + "  Delete"))
          {
            Console.WriteLine("Deleteing " + _spritexPart.Name);
            Spritex.Parts.Data.Remove(_spritexPart.Name);
            ImGui.CloseCurrentPopup();
          }
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Edit + "  Change region"))
          {
            ChangePart = _spritexPart;
            renderer.Editor.GetSubEntity<SpritexView>().UnEdit();
            ImGui.CloseCurrentPopup();
          }
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Clone + "  Duplicate"))
          {
            ImGui.CloseCurrentPopup();
          }
          ImGui.EndPopup();
        }
      }
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.User;
    }
    protected override void OnChangeName(string old, string now)
    {
      Spritex._sheet.Spritexes.ChangeKey(old, now);
    } 
  }
}
