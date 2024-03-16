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
    public List<Level> Levels = new List<Level>();
    public Dictionary<string, Sheet> SpriteSheets = new Dictionary<string, Sheet>();

    public World() {}
    public void AddLevel() {}
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
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
      public Point Size = new Point();
      public Vector2 Position = new Vector2();
      public RectangleF Bounds { get => new RectangleF(Position.X, Position.Y, Size.X, Size.Y); }
      public List<Layer> Layers = new List<Layer>();

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

