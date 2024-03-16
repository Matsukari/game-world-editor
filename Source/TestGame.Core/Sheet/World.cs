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

    public World() {}
    public void AddLevel() {}
    public void AddSheet(Sheet sheet) { SpriteSheets.Add(sheet.Name, sheet); }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    Sheet _openSheet = null;
    Sheet _selectedSheet = null;
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      base.RenderImGui(renderer);
      ImGui.Begin(IconFonts.FontAwesome5.ObjectGroup + " World Lister");
      var stack = new System.Numerics.Vector2(0, 0);
      var levelFlags = ImGuiTreeNodeFlags.None;
      if (Levels.Count() > 0) levelFlags = ImGuiTreeNodeFlags.DefaultOpen;
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.ObjectGroup + " Levels", levelFlags))
      {
        var size = 200;
        stack.Y += size;
        ImGui.BeginChild("levels-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        foreach (var level in Levels)
        {
          if (ImGui.MenuItem(level.Name))
          {
            // scroll to level
            CurrentLevel = level;
          }
        }
        ImGui.EndChild();
      }
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.Th + " SpriteSheets", ImGuiTreeNodeFlags.DefaultOpen))
      {
        var size = 100;
        stack.Y += size;
        ImGui.BeginChild("spritesheets-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        ImGui.Indent();
        foreach (var sheet in SpriteSheets)
        {
          if (ImGui.MenuItem(sheet.Key)) 
          {
            _selectedSheet = sheet.Value;
          }
        }
        ImGui.Unindent();
        ImGui.EndChild();

        if (_selectedSheet != null)
        {
          var previewHeight = 100;
          float ratio = (ImGui.GetWindowWidth()-ImGui.GetStyle().WindowPadding.X) / previewHeight;

          var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(_selectedSheet.Texture);
          ImGui.Image(texture, new System.Numerics.Vector2(ImGui.GetWindowWidth(), previewHeight*ratio));
          if (_openSheet == null && ImGui.IsItemHovered())
          {
            _openSheet = _selectedSheet;
          }
        }
      }
      ImGui.End();

      FocusFactor = _openSheet == null;
      if (_openSheet != null)
      {
        ImGui.Begin("sheet-picker", ImGuiWindowFlags.NoDecoration);
        ImGui.SetWindowSize(new System.Numerics.Vector2(450, 450));
        ImGui.SetWindowPos(new System.Numerics.Vector2(0f, Screen.Height-ImGui.GetWindowHeight()-28));
        ImGui.SetWindowFocus();
       
        var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(_openSheet.Texture);
        ImGui.GetWindowDrawList().AddImage(texture, ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize());
        if (!new RectangleF(ImGui.GetWindowPos(), ImGui.GetWindowSize()).Contains(ImGui.GetMousePos())) _openSheet = null;
        ImGui.End(); 
      }
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
      public Point Size = new Point();
      public Vector2 Position = new Vector2();
      public RectangleF Bounds { get => new RectangleF(Position.X, Position.Y, Size.X, Size.Y); }
      public List<Layer> Layers = new List<Layer>();
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
        Dictionary<int, IPropertied> _tiles = new Dictionary<int, IPropertied>();

        public int GetTile(int x, int y) => y * TilesQuantity.X + x;
        public Point GetTile(int coord) => new Point(coord % TilesQuantity.X, coord / TilesQuantity.X); 
        public void ReplaceTile(int x, int y, Propertied tile)
        {
          if (x < 0 || x >= TilesQuantity.X || y < 0 || y >= TilesQuantity.Y || (!(tile is Tile || tile is Spritex))) return;
          _tiles[GetTile(x, y)] = tile;
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

