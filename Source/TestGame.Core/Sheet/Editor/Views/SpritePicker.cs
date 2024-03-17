using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;
using Num = System.Numerics;

namespace Raven.Sheet
{
  public class SheetPickerData
  {
    public Sheet Sheet;
    public Vector2 Position = new Vector2();
    public float Zoom = 1;
    public SheetPickerData(Sheet sheet) => Sheet = sheet;
  }
  public class SpritePicker
  {
    public List<SheetPickerData> Sheets = new List<SheetPickerData>();
    public SheetPickerData OpenSheet;
    public SheetPickerData SelectedSheet;
    public IPropertied SelectedSprite;
    public RectangleF Bounds = new RectangleF(0, 0, 1, 1);
    Vector2 _initialPosition = Vector2.Zero;
    public void Draw(Editor editor, World world)
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite is Tile tile)
      {
        var min = tile.Region.Location.ToVector2() / Bounds.Size;
        var max = (tile.Region.Location + tile.Region.Size).ToVector2() / Bounds.Size;

        ImGui.GetForegroundDrawList().AddImage(
            Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(tile.Texture),
            rawMouse - tile.Region.GetHalfSize().ToNumerics(), 
            rawMouse + tile.Region.Size.ToVector2().ToNumerics() - tile.Region.GetHalfSize().ToNumerics() + new Num.Vector2(16),
            min.ToNumerics(), max.ToNumerics());

        if (Nez.Input.LeftMouseButtonPressed && !input.IsImGuiBlocking)
        {
          var tilelayer = (world.Levels[0].Layers[0] as World.Level.TileLayer);
          tilelayer.Tiles[tilelayer.GetTileFromWorld(editor.Scene.Camera.MouseToWorldPoint())] = new TileInstance(tile);
        }
      }

      if (OpenSheet == null) return ;
      ImGui.Begin("sheet-picker", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove);
      ImGui.SetWindowSize(new System.Numerics.Vector2(450, 450));
      ImGui.SetWindowPos(new System.Numerics.Vector2(0f, Screen.Height-ImGui.GetWindowHeight()-28));
      ImGui.SetWindowFocus();
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();

      var mouse = MouseToPickerPoint(OpenSheet);
      if (ImGui.BeginTabBar("sheet-picker-tab"))
      {
        if (ImGui.BeginTabItem("Tiles"))
        {
          var tiledMouse = rawMouse;
          tiledMouse.X = (int)(tiledMouse.X / OpenSheet.Sheet.TileWidth) * OpenSheet.Sheet.TileWidth;
          tiledMouse.Y = (int)(tiledMouse.Y / OpenSheet.Sheet.TileHeight) * OpenSheet.Sheet.TileHeight;
          ImGui.GetForegroundDrawList().AddRect(
              tiledMouse.ToVector2().ToNumerics(), 
              (tiledMouse + OpenSheet.Sheet.TileSize.ToVector2()).ToNumerics(), 
              editor.ColorSet.SpriteRegionActiveOutline.ToImColor());
          if (input.IsDragFirst) 
          {
          }
          else if (input.IsDrag)
          {
          }
          else if (input.IsDragLast)
          {
            SelectedSprite = OpenSheet.Sheet.GetTileData(OpenSheet.Sheet.GetTileIdFromWorld(mouse.X, mouse.Y));
            Console.WriteLine("Got: " + mouse.X + " " + mouse.Y);
          }
        }
        if (ImGui.BeginTabItem("Spritexes"))
        {
        }
      }
      HandleMoveZoom();



      var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(OpenSheet.Sheet.Texture);
      ImGui.GetWindowDrawList().AddImage(texture, 
          Bounds.Location.ToNumerics(), 
          Bounds.Location.ToNumerics() + Bounds.Size.ToNumerics(), 
          GetUvMin(OpenSheet), GetUvMax(OpenSheet));

      if (!Bounds.Contains(ImGui.GetMousePos()) && !input.IsDrag) OpenSheet = null;
      ImGui.End(); 
    }
    void HandleMoveZoom()
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var mouse = ImGui.GetMousePos();
      // zooms
      if (ImGui.GetIO().MouseWheel != 0)
      {
        float zoomFactor = 1.2f;
        if (ImGui.GetIO().MouseWheel < 0) 
        {
          if (OpenSheet.Zoom > 1f) zoomFactor = 1/zoomFactor;
          else zoomFactor = 1f;
        }
        var zoom = Math.Clamp(OpenSheet.Zoom * zoomFactor, 1f, 10f);
        var delta = (MouseToPickerPoint(OpenSheet) - OpenSheet.Position) * (zoomFactor - 1);
        if (zoomFactor != 1f) OpenSheet.Position += delta;
        OpenSheet.Zoom = zoom;
      }
      if (input.IsDragFirst)
      {
        _initialPosition = OpenSheet.Position;
      }
      if (input.IsDrag && input.MouseDragButton == 2) 
      {
        OpenSheet.Position = _initialPosition + (input.MouseDragStart - ImGui.GetIO().MousePos);        
      } 
      OpenSheet.Position.X = Mathf.Clamp(OpenSheet.Position.X, 0f, Bounds.Width * (OpenSheet.Zoom-1f));
      OpenSheet.Position.Y = Mathf.Clamp(OpenSheet.Position.Y, 0f, Bounds.Height * (OpenSheet.Zoom-1f));
    }
    public System.Numerics.Vector2 GetUvMin(SheetPickerData state) => (state.Position / Bounds.Size / state.Zoom).ToNumerics(); 
    public System.Numerics.Vector2 GetUvMax(SheetPickerData state) => ((state.Position + Bounds.Size) / Bounds.Size / state.Zoom).ToNumerics();

    // Convert mouse position relative to picker's bounds
    public Vector2 MouseToPickerPoint(SheetPickerData state)
    {
      var mouse = ImGui.GetMousePos();
      var pos = mouse - Bounds.Location;
      pos *= new Vector2(state.Zoom, state.Zoom);
      pos += state.Position;
      return pos;
    }
  }
}
