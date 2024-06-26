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
    public event Action<object> OnSelectRightMouseClick;
    public event Action OnLeave;

    public bool isPickOnlyTile = false;
    public bool IsHoverSelected = false;
    public bool EnableReselect = true;
    public bool PreviewScenes = true;

    bool _firstEnter = true;

    Vector2 _initialMouseOnDrag = Vector2.Zero;
    public virtual void OnHandleSelectedSprite()
    {
    }
    public virtual void OnHandleSelectedSpriteInside()
    {
    }
    public virtual void OutRender()
    {

    }
    public void Draw(RectangleF preBounds, EditorColors colors)
    {
      var input = Core.GetGlobalManager<InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      OutRender();

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
      mouse -= Bounds.Location;
      mouse -= OpenSheet.Position;
      mouse /= sheetZoom;

      // Convenicne for translating raw data to renderable position
      RectangleF TranslateToWorld(RectangleF rect)
      {
        var worldTile = rect;
        worldTile.Location *= sheetZoom;
        worldTile.Location += OpenSheet.Position;
        worldTile.Location += Bounds.Location;
        worldTile.Size *= sheetZoom;
        return worldTile;
      }
      IsHoverSelected = false;
      if (ImGui.BeginTabBar("sheet-picker-tab"))
      {
        // Show tiles
        if (ImGui.BeginTabItem("Tiles"))
        {


          // Draws the spritesheet with zoom and fofset
          var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(OpenSheet.Sheet.Texture);

          ImGuiUtils.DrawImage(ImGui.GetWindowDrawList(), OpenSheet.Sheet.Texture, 
              new RectangleF(Vector2.Zero, OpenSheet.Sheet.Size), 
              (Bounds.Location + OpenSheet.Position).ToNumerics(),
              (OpenSheet.Sheet.Size * OpenSheet.Zoom).ToNumerics(), 
              Vector2.Zero.ToNumerics(), 
              0f, 
              Color.White.ToImColor()); 

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
              if (sprite.Region.Contains(mouse)) 
              {
                IsHoverSelected = true;
              }
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
          HandleMoveZoom();

          // Multiple selection (rectangle) 
          if (input.IsDragFirst && (EnableReselect || (!IsHoverSelected)) && Nez.Input.LeftMouseButtonDown)
          {
            _initialMouseOnDrag = ImGui.GetMousePos();
            SelectedSprite = null;
          }
          else if (input.IsDrag && _initialMouseOnDrag != Vector2.Zero)
          {
            var area = input.MouseDragArea;
            area.Location = _initialMouseOnDrag;
            area.Size = ImGui.GetMousePos() - _initialMouseOnDrag;
            area = area.AlwaysPositive();
            area.Location -= Bounds.Location;
            area.Location -= OpenSheet.Position;
            area.Location /= OpenSheet.Zoom;
            area.Size /= OpenSheet.Zoom;

            var rectTile = new RectangleF();
            rectTile.Location = mouse;
            rectTile.Size = OpenSheet.Sheet.TileSize.ToVector2();
            try 
            {
              if (SelectedSprite is Sprite sprite) sprite.Rectangular(area);
              else if (SelectedSprite == null) SelectedSprite = new Sprite(rectTile.RoundLocationFloor(OpenSheet.Sheet.TileSize), OpenSheet.Sheet);
            } 
            catch (Exception) {}
          }
          else if (input.IsDragLast) _initialMouseOnDrag = Vector2.Zero;

          ImGui.EndTabItem();
        }
        if (ImGui.BeginTabItem("Animations"))
        {
          var size = new Vector2(50, 50);
          foreach (var anim in OpenSheet.Sheet.Animations)  
          {
            if (anim.TotalFrames <= 0) continue;
            var frame = anim.Frames.First() as SpriteAnimationFrame;
            ImGuiUtils.DrawImage(frame.Sprite, size.ToNumerics());
            ImGui.SameLine();

            ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), colors.PickHoverOutline.ToImColor());

            if (ImGui.IsItemHovered()) 
            {
              if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
              {
                SelectedSprite = anim;
                Console.WriteLine("Selected " + SelectedSprite.GetType().Name);
              }
              if (SelectedSprite is AnimatedSprite)
                IsHoverSelected = true;
            }
            else if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && OnSelectRightMouseClick != null) OnSelectRightMouseClick(anim);


            if (RectangleF.FromMinMax(ImGui.GetItemRectMin(), ImGui.GetItemRectMax()).Contains(ImGui.GetMousePos()))
            {
              ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), colors.PickFill.ToImColor());              
            }
            if (SelectedSprite is Animation animation && animation.Name == anim.Name) 
              ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), colors.PickSelectedOutline.ToImColor());

          }
          ImGui.EndTabItem();
        }
        if (!isPickOnlyTile && ImGui.BeginTabItem("Scenes"))
        {
          ImGui.SameLine();
          Widget.ImGuiWidget.ToggleButton(IconFonts.FontAwesome5.ThLarge, ref PreviewScenes);
          if (PreviewScenes)
          {
            var scenePos = ImGui.GetCursorScreenPos();
            foreach (var spriteScene in OpenSheet.Sheet.SpriteScenees)
            {
              var size = new Vector2(50, 50);
              var sceneScaleRatio = size / spriteScene.EnclosingBounds.Size;

              ImGui.GetWindowDrawList().AddRect(scenePos, (scenePos + size).ToNumerics(), colors.PickHoverOutline.ToImColor());

              foreach (var part in spriteScene.Parts)
              {
                var partPos = part.Transform.Position * sceneScaleRatio;
                partPos -= spriteScene.EnclosingBounds.Location * sceneScaleRatio;
                var partSize = part.Transform.Scale * sceneScaleRatio * part.SourceSprite.Region.Size.ToVector2();
                ImGuiUtils.DrawImage(ImGui.GetWindowDrawList(), part.SourceSprite, (scenePos + partPos).ToNumerics(), partSize.ToNumerics());
              }
              var sceneBounds = new RectangleF(scenePos, size);

              if (sceneBounds.Contains(ImGui.GetMousePos()))
              {
                ImGui.GetWindowDrawList().AddRectFilled(scenePos, (scenePos + size).ToNumerics(), colors.PickFill.ToImColor());

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                  SelectedSprite = spriteScene;
              }

              if (SelectedSprite is SpriteScene scene && scene.Name == spriteScene.Name)
                ImGui.GetWindowDrawList().AddRect(scenePos, (scenePos + size).ToNumerics(), colors.PickSelectedOutline.ToImColor());

              scenePos.X += size.X;
              if (scenePos.X >= ImGui.GetCursorScreenPos().X + ImGui.GetWindowSize().X)
                scenePos = new System.Numerics.Vector2(ImGui.GetCursorScreenPos().X, scenePos.Y + size.Y);
            }
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
          OnHandleSelectedSpriteInside();

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
    public float MinZoom = 0.000001f;
    public float MaxZoom = 100f;
    public float ZoomFactor = 1.2f;
    void HandleMoveZoom()
    {
      var input = Core.GetGlobalManager<InputManager>();
      // zooms
      if (ImGui.GetIO().MouseWheel != 0)
      {
        float zoomFactor = ZoomFactor;
        if (ImGui.GetIO().MouseWheel < 0) 
        {
          if (OpenSheet.Zoom > MinZoom) zoomFactor = 1/zoomFactor;
          else zoomFactor = MinZoom;
        }
        var zoom = Math.Clamp(OpenSheet.Zoom * zoomFactor, MinZoom, MaxZoom);
        var mouse = MouseToPickerPoint(OpenSheet.Zoom, OpenSheet.Position);
        var delta = (OpenSheet.Position - mouse) * (zoomFactor - 1);
        if (zoomFactor != 1f) OpenSheet.Position += delta;
        OpenSheet.Zoom = zoom;
      }
      if (input.IsDragFirst)
      {
        _initialPosition = OpenSheet.Position;
      }
      if (input.IsDrag && input.MouseDragButton == 2) 
      {
        OpenSheet.Position = _initialPosition - (input.MouseDragStart - ImGui.GetIO().MousePos);        
      } 
    }
    public System.Numerics.Vector2 GetUvMin(SheetPickerData state) => (state.Position / Bounds.Size / state.Zoom).ToNumerics(); 
    public System.Numerics.Vector2 GetUvMax(SheetPickerData state) => ((state.Position + Bounds.Size) / Bounds.Size / state.Zoom).ToNumerics();

    // Convert mouse position relative to picker's bounds
    public Vector2 MouseToPickerPoint(float zoom, Vector2 offset)
    {
      var mouse = ImGui.GetMousePos();
      var pos = mouse.ToVector2(); 
      pos -= ImGui.GetCursorScreenPos();
      // pos /= zoom;
      // pos += offset;

      return pos;
    }
  }
}
