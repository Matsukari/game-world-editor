using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetPickerData
  {
    public Sheet Sheet;
    public Vector2 Position = new Vector2();
    public float Zoom = 1;
    public uint GridColor = 0;
    public uint HoverTileBorderColor = 0;
    public uint HoverTileFillColor = 0;
    public bool Selected = false;
    public SheetPickerData(Sheet sheet) => Sheet = sheet;
    public SheetPickerData(Sheet sheet, EditorColors colors) 
    {
      Sheet = sheet;
      GridColor = colors.LevelGrid.ToImColor();
      HoverTileBorderColor = colors.PickHoverOutline.ToImColor();
      HoverTileFillColor = colors.PickFill.ToImColor();
    }

  }
  public class SpritePicker
  {
    public List<SheetPickerData> Sheets = new List<SheetPickerData>();
    public SheetPickerData OpenSheet;
    public SheetPickerData SelectedSheet;
    public object SelectedSprite;
    public RectangleF Bounds = new RectangleF(0, 0, 1, 1);
    Vector2 _initialPosition = Vector2.Zero;
    List<RectangleF> _tiles = new List<RectangleF>();
    public Action HandleSelectedSprite = null;
    public Action OnSelectRightMouseClick = null;

    public bool isPickOnlyTile = false;
    public bool IsHoverSelected = false;
    public bool EnableReselect = true;

    Vector2 _initialMouseOnDrag = Vector2.Zero;
    public void Draw(RectangleF preBounds)
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite != null && Nez.Input.RightMouseButtonReleased) SelectedSprite = null;

      if (OpenSheet == null) 
      {
        if (SelectedSprite != null && HandleSelectedSprite != null && _initialMouseOnDrag == Vector2.Zero)
          HandleSelectedSprite.Invoke();
        return;
      }

      // Begin picker
      ImGui.Begin("sheet-picker", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove);
      ImGui.SetWindowSize(preBounds.Size.ToNumerics());
      ImGui.SetWindowPos(new System.Numerics.Vector2(preBounds.X, preBounds.Y));
      ImGui.SetWindowFocus();

      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();
  
      // Window draws 2 vertical components; use only partitioned size from total bounds
      var totalBounds = Bounds;
      var barSize = new Vector2(0, 25);

      // The bounds of the picker itself
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

      // Convenicne for translating raw data to renderable position
      RectangleF TranslateToWorld(RectangleF rect)
      {
        var worldTile = rect;
        worldTile.Location *= sheetZoom;
        worldTile.Location -= OpenSheet.Position;
        worldTile.Location += Bounds.Location;
        worldTile.Size *= sheetZoom;
        return worldTile;
      }

      if (ImGui.BeginTabBar("sheet-picker-tab"))
      {
        // Show tiles
        if (ImGui.BeginTabItem("Tiles"))
        {
          // Highlights tile on mouse hvoer
          foreach (var rectTile in _tiles)
          {
            // Translate to world
            var worldTile = TranslateToWorld(rectTile);

            // Draws grid
            if (Bounds.Contains(worldTile))
            {
              ImGui.GetForegroundDrawList().AddRect(
                  (worldTile.Location).ToNumerics(), 
                  (worldTile.Location + worldTile.Size).ToNumerics(), 
                  OpenSheet.GridColor);
            }
            // Draws highlight under mouse
            if (rectTile.Contains(mouse) && !input.IsDrag)
            {
              ImGui.GetForegroundDrawList().AddRect(
                  (worldTile.Location).ToNumerics(), 
                  (worldTile.Location + worldTile.Size).ToNumerics(), 
                  OpenSheet.HoverTileBorderColor);
            }
          }
          // Highlights selected sprite
          {
            if (SelectedSprite is Sprite sprite)
            {
              IsHoverSelected = false;
              if (sprite.Region.Contains(mouse)) IsHoverSelected = true;
              var regionWorld = TranslateToWorld(sprite.Region.ToRectangleF());
              ImGui.GetForegroundDrawList().AddRectFilled(
                  (regionWorld.Location).ToNumerics(), 
                  (regionWorld.Location + regionWorld.Size).ToNumerics(), 
                  OpenSheet.HoverTileFillColor);               
            }
          }
          var mouseDragArea = new RectangleF();
          mouseDragArea.Location = _initialMouseOnDrag;

          // Multiple selection (rectangle) 
          if (input.IsDragFirst && (EnableReselect || !IsHoverSelected))
          {
            _initialMouseOnDrag = mouse;
            SelectedSprite = null;
          }
          else if (input.IsDrag && _initialMouseOnDrag != Vector2.Zero)
          {
            mouseDragArea.Size = (mouse - _initialMouseOnDrag);
            mouseDragArea = mouseDragArea.AlwaysPositive();

            var bounds = Bounds;
            bounds.Location = new Vector2();
            bounds.Size /= sheetScale;

            var rectTile = new RectangleF();
            rectTile.Location = mouse;
            rectTile.Size = OpenSheet.Sheet.TileSize.ToVector2();

            if (mouseDragArea.Intersects(bounds))
            {
              var tiled = OpenSheet.Sheet.GetTileCoordFromWorld(rectTile.X, rectTile.Y);
              // Prevent going out of bounds
              tiled.X = Math.Min(tiled.X, OpenSheet.Sheet.Tiles.X-1);
              tiled.Y = Math.Min(tiled.Y, OpenSheet.Sheet.Tiles.Y-1); 

              if (SelectedSprite is Sprite sprite) sprite.Rectangular(OpenSheet.Sheet.GetTileId(tiled.X, tiled.Y));
              else if (SelectedSprite == null) SelectedSprite = new Sprite(rectTile.RoundLocationFloor(OpenSheet.Sheet.TileSize), OpenSheet.Sheet);
            }
          }
          else if (input.IsDragLast) _initialMouseOnDrag = Vector2.Zero;


          HandleMoveZoom();
          // Draws the spritesheet with zoom and fofset
          var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(OpenSheet.Sheet.Texture);
          ImGui.GetWindowDrawList().AddImage(texture, 
              Bounds.Location.ToNumerics(), 
              Bounds.Location.ToNumerics() + Bounds.Size.ToNumerics(), 
              GetUvMin(OpenSheet), GetUvMax(OpenSheet));

          ImGui.EndTabItem();
        }
        var grid = false;
        if (!isPickOnlyTile && ImGui.BeginTabItem("Spritexes"))
        {
          Widget.ImGuiWidget.ButtonSetFlat(new List<(string, Action)>{
              (IconFonts.FontAwesome5.ThLarge, ()=>{grid=true;}),
              (IconFonts.FontAwesome5.GripLines, ()=>{grid=false;}),
          });
          if (grid)
          {
          }
          else 
          {
            ImGui.Indent();
            foreach (var spritex in OpenSheet.Sheet.Spritexes)
            {
              if (ImGui.MenuItem(spritex.Name))
              {
                SelectedSprite = spritex;
              }
            }
            ImGui.Unindent();
          }
          ImGui.EndTabItem();
        }
      }
      if (SelectedSprite != null && HandleSelectedSprite != null && _initialMouseOnDrag == Vector2.Zero)
          HandleSelectedSprite.Invoke();

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
