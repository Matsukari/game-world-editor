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

    void HandleSelectedSprite(Editor editor, World world)
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
            rawMouse - tile.Region.GetHalfSize().ToNumerics() + tile.Region.Size.ToVector2().ToNumerics(),
            min.ToNumerics(), max.ToNumerics(), new Color(1f, 1f, 1f, 0.3f).ToImColor());

        if (Nez.Input.LeftMouseButtonPressed && !input.IsImGuiBlocking)
        {
          var tilelayer = (world.Levels[0].Layers[0] as World.Level.TileLayer);
          var tileApprox = editor.Scene.Camera.MouseToWorldPoint(); 
          tilelayer.ReplaceTile(tilelayer.GetTileCoordFromWorld(tileApprox), new TileInstance(tile));
        }
      }
    }
    public void Draw(Editor editor, World world)
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();


      if (OpenSheet == null) 
      {
        HandleSelectedSprite(editor, world);
        return;
      }

      // Begin picker
      ImGui.Begin("sheet-picker", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove);
      ImGui.SetWindowSize(new System.Numerics.Vector2(450, 450));
      ImGui.SetWindowPos(new System.Numerics.Vector2(0f, Screen.Height-ImGui.GetWindowHeight()-28));
      ImGui.SetWindowFocus();
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();
      var totalBounds = Bounds;
      var barSize = new Vector2(0, 25);
      Bounds.Location += barSize;
      Bounds.Size -= barSize;
      // Sync tiles
      if (_tiles.Count() == 0) RebuildTiles();

      // The ratio between the drawn size and it's actual size (in raw texture)
      var sheetScale = Bounds.Size / OpenSheet.Sheet.Size;
      var sheetZoom = sheetScale * OpenSheet.Zoom;

      // The mouse relative to the picker's bounds, plus zoom and offset; 
      // this is relative to the actual size of the texture as opoposed to the rendered bounds
      var mouse = rawMouse - Bounds.Location;  
      mouse = mouse + OpenSheet.Position;
      mouse /= sheetZoom;

      // Highlights selection
      if (SelectedSprite is Tile tile)
      {
      }

      if (ImGui.BeginTabBar("sheet-picker-tab"))
      {
        if (ImGui.BeginTabItem("Tiles"))
        {
          // Highlights tile on mouse hvoer
          foreach (var rectTile in _tiles)
          {
            // Translate to world
            var worldTile = rectTile;
            worldTile.Location *= sheetZoom;
            worldTile.Location -= OpenSheet.Position;
            worldTile.Location += Bounds.Location;
            worldTile.Size *= sheetZoom;
            if (totalBounds.Contains(worldTile))
            {
              ImGui.GetForegroundDrawList().AddRect(
                  (worldTile.Location).ToNumerics(), 
                  (worldTile.Location + worldTile.Size).ToNumerics(), 
                  editor.ColorSet.SpriteRegionInactiveOutline.ToImColor());
            }
            if (rectTile.Contains(mouse))
            {
              ImGui.GetForegroundDrawList().AddRect(
                  (worldTile.Location).ToNumerics(), 
                  (worldTile.Location + worldTile.Size).ToNumerics(), 
                  editor.ColorSet.SpriteRegionActiveOutline.ToImColor());
            }
          }
          if (input.IsDragFirst) 
          {
          }
          else if (input.IsDrag)
          {
          }
          else if (input.IsDragLast)
          {
            foreach (var rectTile in _tiles)
            {
              if (rectTile.Contains(mouse))
              {
                SelectedSprite = OpenSheet.Sheet.GetTileData(OpenSheet.Sheet.GetTileIdFromWorld(rectTile.X, rectTile.Y));
              }
            }
          }
        }
        if (ImGui.BeginTabItem("Spritexes"))
        {
        }
      }
      HandleMoveZoom();
      HandleSelectedSprite(editor, world);

      // Draws the spritesheet with zoom and fofset
      var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(OpenSheet.Sheet.Texture);
      ImGui.GetWindowDrawList().AddImage(texture, 
          Bounds.Location.ToNumerics(), 
          Bounds.Location.ToNumerics() + Bounds.Size.ToNumerics(), 
          GetUvMin(OpenSheet), GetUvMax(OpenSheet));

      // Mouse is outside the enlargened picker
      if (!totalBounds.Contains(ImGui.GetMousePos()) && !input.IsDrag) 
      {
        OpenSheet = null;
        _tiles.Clear();
      }
      ImGui.End(); 
    }
    void RebuildTiles()
    {
      for (int x = 0; x < OpenSheet.Sheet.Tiles.X; x++)
      {
        for (int y = 0; y < OpenSheet.Sheet.Tiles.Y; y++)
        {
          _tiles.Add(new RectangleF(x * OpenSheet.Sheet.TileWidth, y * OpenSheet.Sheet.TileHeight, OpenSheet.Sheet.TileWidth, OpenSheet.Sheet.TileHeight));
        }
      }
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
