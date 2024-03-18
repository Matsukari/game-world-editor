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
    List<RectangleF> _tiles = new List<RectangleF>();
    public void Draw(Editor editor, World world)
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite is Tile tile)
      {
        var min = tile.Region.Location.ToVector2() / tile.Texture.GetSize();
        var max = (tile.Region.Location + tile.Region.Size).ToVector2() / tile.Texture.GetSize();

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
      if (_tiles.Count() == 0)
      {
        for (int x = 0; x < OpenSheet.Sheet.Tiles.X; x++)
        {
          for (int y = 0; y < OpenSheet.Sheet.Tiles.Y; y++)
          {
            _tiles.Add(new RectangleF(x * OpenSheet.Sheet.TileWidth, y * OpenSheet.Sheet.TileHeight, OpenSheet.Sheet.TileWidth, OpenSheet.Sheet.TileHeight));
          }
        }
      }

      var sheetScale = Bounds.Size / OpenSheet.Sheet.Size;
      // var mouse = MouseToPickerPoint(OpenSheet);
      // mouse /= sheetScale;
      var mouse = rawMouse - Bounds.Location;  
      mouse = mouse + OpenSheet.Position;
      mouse /= OpenSheet.Zoom * sheetScale;

      if (ImGui.BeginTabBar("sheet-picker-tab"))
      {
        if (ImGui.BeginTabItem("Tiles"))
        {
          var sheetZoom = sheetScale * OpenSheet.Zoom;
          var tiledMouse = mouse;
          var tileSize = OpenSheet.Sheet.TileSize.ToVector2() ;
          tiledMouse -= OpenSheet.Position;
          tiledMouse /= new Vector2(OpenSheet.Zoom, OpenSheet.Zoom);
          tiledMouse.X = ((int)(tiledMouse.X / tileSize.X) * tileSize.X); 
          tiledMouse.Y = ((int)(tiledMouse.Y / tileSize.Y) * tileSize.Y); 

          // foreach (var rectTile in _tiles)
          // {
          //   var worldTile = rectTile;
          //   worldTile.Location += Bounds.Location;
          //   worldTile.Location *= sheetScale;
          //   worldTile.Size *= sheetZoom;
          //   if (rectTile.Contains(mouse))
          //   {
          //     ImGui.GetForegroundDrawList().AddRect(
          //         (worldTile.Location).ToNumerics(), 
          //         (worldTile.Location + worldTile.Size).ToNumerics(), 
          //         editor.ColorSet.SpriteRegionActiveOutline.ToImColor());
          //   }
          // }
          ImGui.GetForegroundDrawList().AddRect(
              (Bounds.Location + tiledMouse).ToNumerics(), 
              (Bounds.Location + tiledMouse + OpenSheet.Sheet.TileSize.ToVector2() * sheetZoom).ToNumerics(), 
              editor.ColorSet.SpriteRegionActiveOutline.ToImColor());
          // tiledMouse += OpenSheet.Position.ToNumerics() * sheetZoom.ToNumerics();
          if (input.IsDragFirst) 
          {
          }
          else if (input.IsDrag)
          {
          }
          else if (input.IsDragLast)
          {
            var zoom = OpenSheet.Zoom;
            var mouseRel = rawMouse - Bounds.Location;
            var position = OpenSheet.Position / OpenSheet.Zoom;
            var excess = Bounds.Size - position;
            foreach (var rectTile in _tiles)
            {
              var worldTile = rectTile;
              worldTile.Location += Bounds.Location;
              if (rectTile.Contains(mouse))
              {
                SelectedSprite = OpenSheet.Sheet.GetTileData(OpenSheet.Sheet.GetTileIdFromWorld(rectTile.X, rectTile.Y));
              }
            }
            // var tileCoord = (position + mouseRel / OpenSheet.Zoom) / sheetScale;
            // var tileCoord = (position + excess * (mouseRel / Bounds.Size)) / sheetScale; 
            // Console.WriteLine($"\n Position: {OpenSheet.Position/OpenSheet.Zoom}\n Mouse zoomed: {mouse}\n Mouse relative: {rawMouse-Bounds.Location}\n World {tileCoord},\n Total zoom: {sheetZoom}\n Sheet scale {sheetScale}\n Gui Zoom {OpenSheet.Zoom}\n ");
            // SelectedSprite = OpenSheet.Sheet.GetTileData(OpenSheet.Sheet.GetTileIdFromWorld(tileCoord.X, tileCoord.Y));
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

      if (!Bounds.Contains(ImGui.GetMousePos()) && !input.IsDrag) 
      {
        OpenSheet = null;
        _tiles.Clear();
      }
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
