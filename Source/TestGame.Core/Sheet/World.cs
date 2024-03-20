using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  // <summary>
  // Expands as the levels are painted on
  // </summary>
  public class World : Propertied
  {
    public Point Size { get 
    { 
      var size = new Point();
      foreach (var level in Levels) 
        size += level.Size;
      return size;
    } }
    public Level CurrentLevel = null;
    public List<Level> Levels = new List<Level>();
    public Dictionary<string, Sheet> SpriteSheets = new Dictionary<string, Sheet>();
    public IPropertied SelectedSprite;

    public World() 
    {
      Level level = CreateLevel();
      CurrentLevel = level;
    }
    public Level CreateLevel(string name = "Level default") 
    {
      Level level = new Level(this);
      Level.Layer layer = new Level.TileLayer(level, 16, 16);
      level.Name = name;
      level.Layers.Add(layer);
      level.CurrentLayer = layer;
      Levels.Add(level);
      return level;
    }
    public void AddSheet(Sheet sheet) { SpriteSheets.Add(sheet.Name, sheet); }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    public void DrawArtifacts(Batcher batcher, Camera camera, Editor editor, GuiData gui)
    {
      if (CurrentLevel != null)
      {
        batcher.DrawRectOutline(camera, CurrentLevel.Bounds, editor.ColorSet.SpriteRegionActiveOutline);
      }
    }
    SpritePicker _spritePicker = new SpritePicker();
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      base.RenderImGui(renderer);
      ImGui.Begin(IconFonts.FontAwesome5.ObjectGroup + " World Lister");
      var stack = new System.Numerics.Vector2(0, 0);
      var levelFlags = ImGuiTreeNodeFlags.None;
      if (Levels.Count() > 0) levelFlags = ImGuiTreeNodeFlags.DefaultOpen;
      if (_spritePicker.Sheets.Count() != SpriteSheets.Count())
      {
        _spritePicker.Sheets.Clear();
        foreach (var sheet in SpriteSheets) _spritePicker.Sheets.Add(new SheetPickerData(sheet.Value));
      }

      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.ObjectGroup + " Levels", levelFlags))
      {
        var size = 200;
        stack.Y += size;
        ImGui.BeginChild("levels-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        ImGui.Indent();
        foreach (var level in Levels)
        {
          if (ImGui.MenuItem(level.Name))
          {
            // scroll to level
            CurrentLevel = level;
          }
        }
        ImGui.Unindent();
        ImGui.EndChild();
      }
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.Th + " SpriteSheets", ImGuiTreeNodeFlags.DefaultOpen))
      {
        var size = 100;
        stack.Y += size;
        ImGui.BeginChild("spritesheets-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        ImGui.Indent();
        foreach (var sheet in _spritePicker.Sheets)
        {
          if (ImGui.MenuItem(sheet.Sheet.Name)) 
          {
            _spritePicker.SelectedSheet= sheet;
          }
        }
        ImGui.Unindent();
        ImGui.EndChild();

        if (_spritePicker.SelectedSheet != null)
        {
          float previewHeight = 100;
          float previewWidth = ImGui.GetWindowWidth()-ImGui.GetStyle().WindowPadding.X*2-3; 
          float ratio = (previewWidth) / previewHeight;

          // Draws the selected spritesheet
          var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(_spritePicker.SelectedSheet.Sheet.Texture);
          ImGui.Image(texture, new System.Numerics.Vector2(previewWidth, previewHeight*ratio), 
              _spritePicker.GetUvMin(_spritePicker.SelectedSheet), _spritePicker.GetUvMax(_spritePicker.SelectedSheet));
          if (_spritePicker.OpenSheet == null && ImGui.IsItemHovered())
          {
            _spritePicker.OpenSheet = _spritePicker.SelectedSheet;
          }
        }
      }
      ImGui.End();

      FocusFactor = _spritePicker.OpenSheet == null;
      _spritePicker.Draw(renderer.Editor, this);
      SelectedSprite = _spritePicker.SelectedSprite;
      if (_spritePicker.SelectedSprite != null && Nez.Input.RightMouseButtonReleased) _spritePicker.SelectedSprite = null;
    }
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{Size.X} px");
      ImGui.LabelText("Height", $"{Size.Y} px");
      ImGui.EndDisabled();
    }

    public class Level : Propertied
    {
      World _world;
      public Point Size = new Point(96, 96);
      public Vector2 Position = new Vector2();
      public RectangleF Bounds { get => new RectangleF(Position.X, Position.Y, Size.X, Size.Y); }
      public List<Layer> Layers = new List<Layer>();
      public Layer CurrentLayer = null;
      public Level(World world)
      {
        _world = world;
      }
      public override string GetIcon()
      {
        return IconFonts.FontAwesome5.LayerGroup;
      }
      protected override void OnRenderAfterName(PropertiesRenderer renderer)
      {
        ImGui.BeginDisabled();
        ImGui.LabelText("Width", $"{Size.X} px");
        ImGui.LabelText("Height", $"{Size.Y} px");
        ImGui.EndDisabled();
      }

      public class Layer : Propertied
      {
        public Level Level;
        public float Opacity = 1f;
        public Vector2 Offset = new Vector2();
        public Layer(Level level)
        {
          Level = level;
        }
        public virtual void Draw(Batcher batcher, Camera camera)
        {
        }
        protected override void OnRenderAfterName(PropertiesRenderer renderer)
        {
          var offset = Offset.ToNumerics();
          if (ImGui.InputFloat2("Offset", ref offset)) Offset = offset;
          ImGui.InputFloat("Opacity", ref Opacity);
        }
      }
      // <summary>
      // Any tile size. Can accept tiles of arbitrary sizes; scales them to fit this layer's tile size
      // </summary> 
      public class TileLayer : Layer
      {
        public TileLayer(Level level, int w, int h) : base(level) 
        {
          TileWidth = w;
          TileHeight = h;
        }
        public int TileWidth;
        public int TileHeight;
        public Point TileSize { get => new Point(TileWidth, TileHeight); }
        public Point TilesQuantity { get => new Point(Level.Size.X/TileWidth, Level.Size.Y/TileHeight); }
        public List<Point> TileHighlights = new List<Point>();
        public Dictionary<Point, InstancedSprite> Tiles { get => _tiles; }
        Dictionary<Point, InstancedSprite> _tiles = new Dictionary<Point, InstancedSprite>();

        public Point GetTileCoordFromWorld(Vector2 point) => new Point(
            (int)(point.X - Level.Bounds.Location.X) / TileWidth, 
            (int)(point.Y - Level.Bounds.Location.Y) / TileHeight);
        public Point GetTile(int coord) => new Point(coord % TilesQuantity.X, coord / TilesQuantity.X); 
        public bool IsTileValid(int x, int y) => !(x < 0 || x >= TilesQuantity.X || y < 0 || y >= TilesQuantity.Y); 
        public void ReplaceTile(Point point, InstancedSprite tile) => ReplaceTile(point.X, point.Y, tile);
        public void ReplaceTile(int x, int y, InstancedSprite tile)
        {
          if (!IsTileValid(x, y)) return;
          _tiles[new Point(x, y)] = tile;
        }
        public override void Draw(Batcher batcher, Camera camera)
        {
          foreach (var block in _tiles)
          {
            var bounds = new RectangleF(block.Key.X*TileWidth, block.Key.Y*TileHeight, TileWidth, TileHeight);
            bounds.Location += Level.Bounds.Location;
            switch (block.Value)
            {
              case TileInstance tile: tile.Draw(batcher, camera, bounds); break;
            }
          }
          foreach (var tile in TileHighlights)
          {
            var bounds = new RectangleF(tile.X*TileWidth, tile.Y*TileHeight, TileWidth, TileHeight);
            bounds.Location += Level.Bounds.Location;
            _tiles[tile].Draw(batcher, camera, bounds);
          }
        }

      }
      public class EntityLayer : Layer
      {
        public EntityLayer(Level level) : base(level) {}
        public List<Spritex> Spritexes;
      } 
    }
  }
}

