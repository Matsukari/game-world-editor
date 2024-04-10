using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven
{
  public class SheetPickerData
  {
    public Sheet Sheet;
    public float Zoom = 1;
    public bool Selected = false;
    public Vector2 Position = new Vector2();
    public SheetPickerData(Sheet sheet) => Sheet = sheet;

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
    public event Action OnSelectRightMouseClick;
    public event Action OnLeave;

    public bool isPickOnlyTile = false;
    public bool IsHoverSelected = false;
    public bool EnableReselect = true;

    bool _firstEnter = true;

    Vector2 _initialMouseOnDrag = Vector2.Zero;
    public virtual void OnHandleSelectedSprite()
    {
    }
    public void Draw(RectangleF preBounds, EditorColors colors)
    {
      var input = Core.GetGlobalManager<InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite != null && Nez.Input.RightMouseButtonReleased) SelectedSprite = null;

      if (OpenSheet == null) 
      {
        if (SelectedSprite != null && _initialMouseOnDrag == Vector2.Zero)
          OnHandleSelectedSprite();
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
      var sheetZoom = OpenSheet.Zoom;

      // The mouse relative to the picker's bounds, plus zoom and offset; 
      // this is relative to the actual size of the texture as opoposed to the rendered bounds
      var mouse = rawMouse.ToVector2();  
      mouse /= sheetZoom;
      mouse -= Bounds.Location;
      mouse -= OpenSheet.Position;

      // Convenicne for translating raw data to renderable position
      RectangleF TranslateToWorld(RectangleF rect)
      {
        var worldTile = rect;
        worldTile.Location += OpenSheet.Position;
        worldTile.Location += Bounds.Location;
        worldTile.Location *= sheetZoom;
        worldTile.Size *= sheetZoom;
        return worldTile;
      }

      if (ImGui.BeginTabBar("sheet-picker-tab"))
      {
        // Show tiles
        if (ImGui.BeginTabItem("Tiles"))
        {


          HandleMoveZoom();
          // Draws the spritesheet with zoom and fofset
          var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(OpenSheet.Sheet.Texture);

          ImGui.GetWindowDrawList().AddImage(texture, 
              (Bounds.Location.ToNumerics() + OpenSheet.Position.ToNumerics()) * OpenSheet.Zoom, 
              (Bounds.Location.ToNumerics() + OpenSheet.Position.ToNumerics() + OpenSheet.Sheet.Size.ToNumerics()) * OpenSheet.Zoom, 
              new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(1, 1));


          // Highlights tile on mouse hvoer
          foreach (var rectTile in _tiles)
          {
            // Translate to world
            var worldTile = TranslateToWorld(rectTile);

            // Draws grid
            ImGui.GetWindowDrawList().AddRect(
                (worldTile.Location).ToNumerics(), 
                (worldTile.Location + worldTile.Size).ToNumerics(), 
                colors.PickHoverOutline.ToImColor());

            // Draws highlight under mouse
            if (rectTile.Contains(mouse) && !input.IsDrag)
            {
              ImGui.GetWindowDrawList().AddRect(
                  (worldTile.Location).ToNumerics(), 
                  (worldTile.Location + worldTile.Size).ToNumerics(), 
                  colors.PickSelectedOutline.ToImColor());
            }
          }
          // Highlights selected sprite
          {
            if (SelectedSprite is Sprite sprite)
            {
              IsHoverSelected = false;
              if (sprite.Region.Contains(mouse)) IsHoverSelected = true;
              var regionWorld = TranslateToWorld(sprite.Region.ToRectangleF());
              ImGui.GetWindowDrawList().AddRectFilled(
                  (regionWorld.Location).ToNumerics(), 
                  (regionWorld.Location + regionWorld.Size).ToNumerics(), 
                  colors.PickFill.ToImColor());               
              ImGui.GetWindowDrawList().AddRect(
                  (regionWorld.Location).ToNumerics(), 
                  (regionWorld.Location + regionWorld.Size).ToNumerics(), 
                  colors.PickSelectedOutline.ToImColor());               
            }
          }
          var mouseDragArea = new RectangleF();
          mouseDragArea.Location = _initialMouseOnDrag;

          // Multiple selection (rectangle) 
          if (input.IsDragFirst && (EnableReselect || !IsHoverSelected) && Nez.Input.LeftMouseButtonDown)
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
            // bounds.Size /= sheetZoom;

            var rectTile = new RectangleF();
            rectTile.Location = mouse;
            rectTile.Size = OpenSheet.Sheet.TileSize.ToVector2();

            var tiled = OpenSheet.Sheet.GetTileCoordFromWorld(rectTile.X, rectTile.Y);
            // Prevent going out of bounds
            tiled.X = Math.Min(tiled.X, OpenSheet.Sheet.Tiles.X-1);
            tiled.Y = Math.Min(tiled.Y, OpenSheet.Sheet.Tiles.Y-1); 

            try 
            {
              if (SelectedSprite is Sprite sprite) sprite.Rectangular(OpenSheet.Sheet.GetTileId(tiled.X, tiled.Y));
              else if (SelectedSprite == null) SelectedSprite = new Sprite(rectTile.RoundLocationFloor(OpenSheet.Sheet.TileSize), OpenSheet.Sheet);
            } catch (Exception) {}
          }
          else if (input.IsDragLast) _initialMouseOnDrag = Vector2.Zero;

          ImGui.EndTabItem();
        }
        var grid = false;
        if (!isPickOnlyTile && ImGui.BeginTabItem("SpriteScenees"))
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
            foreach (var spriteScene in OpenSheet.Sheet.SpriteScenees)
            {
              if (ImGui.MenuItem(spriteScene.Name))
              {
                SelectedSprite = spriteScene;
              }
            }
            ImGui.Unindent();
          }
          ImGui.EndTabItem();
        }
      }
      if (SelectedSprite != null && _initialMouseOnDrag == Vector2.Zero)
          OnHandleSelectedSprite();

      // Mouse is outside the enlargened picker
      if (!totalBounds.Contains(ImGui.GetMousePos()) && !input.IsDrag) 
      {
        if (OnLeave != null)
          OnLeave();
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
      var input = Core.GetGlobalManager<InputManager>();
      var mouse = ImGui.GetMousePos();
      // zooms
      if (ImGui.GetIO().MouseWheel != 0)
      {
        float zoomFactor = 1.2f;
        if (ImGui.GetIO().MouseWheel < 0) 
        {
          if (OpenSheet.Zoom > 0.01) zoomFactor = 1/zoomFactor;
          else zoomFactor = 0.01f;
        }
        var zoom = Math.Clamp(OpenSheet.Zoom * zoomFactor, 0.01f, 10f);
        var delta = (OpenSheet.Position - MouseToPickerPoint(OpenSheet)) * (zoomFactor - 1);
        if (zoomFactor != 1f) OpenSheet.Position += delta;
        OpenSheet.Zoom = zoom;
      }
      if (input.IsDragFirst)
      {
        _initialPosition = OpenSheet.Position;
      }
      if (input.IsDrag && input.MouseDragButton == 2) 
      {
        OpenSheet.Position = _initialPosition - (input.MouseDragStart - ImGui.GetIO().MousePos) / OpenSheet.Zoom;        
      } 
    }
    public System.Numerics.Vector2 GetUvMin(SheetPickerData state) => (state.Position / Bounds.Size / state.Zoom).ToNumerics(); 
    public System.Numerics.Vector2 GetUvMax(SheetPickerData state) => ((state.Position + Bounds.Size) / Bounds.Size / state.Zoom).ToNumerics();

    // Convert mouse position relative to picker's bounds
    public Vector2 MouseToPickerPoint(SheetPickerData state)
    {
      var mouse = ImGui.GetMousePos();
      var pos = mouse.ToVector2(); 
      pos -= Bounds.Location;
      pos /= OpenSheet.Zoom;
      pos += state.Position;

      return pos;
    }
  }
}
