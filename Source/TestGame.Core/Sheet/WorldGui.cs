using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;
using Raven.Sheet.Sprites;

namespace Raven.Sheet
{
  // <summary>
  // Expands as the levels are painted on
  // </summary>
  public enum PaintType { Single, Rectangle, Line, Fill }
  public enum PaintMode { Pen, Eraser }
  public class WorldGui : Propertied
  {
    internal World _world;
    public object SelectedSprite;
    SpritePicker _spritePicker = new SpritePicker();
    public bool IsDrawTileLayerGrid = false;
    public bool IsRandomPaint = false;
    public PaintMode PaintMode = PaintMode.Pen;
    public PaintType PaintType = PaintType.Single;
    public bool CanPaint { get => SelectedSprite != null; }
    internal List<LevelGui> _levelGuis = new List<LevelGui>();
    
    public int SelectedLevel 
    { 
      get => _selectedLevel; 
      set 
      {
        SyncLevelGuis();
        if (value < -1 || value >= _world.Levels.Count) throw new IndexOutOfRangeException();
        _selectedLevel = value; 
        foreach (var levelGui in _levelGuis) levelGui.Selected = false;
        if (value >= 0)
        {
          _levelGuis[SelectedLevel].Selected = true;
          _world.CurrentLevel = _world.Levels[value]; 
        }
      }
    }
    int _selectedLevel = -1;
    Editor _editor;
    public override string Name { get => _world.Name; set => _world.Name = value; }
        
    public WorldGui(Editor editor, World world) 
    {
      _editor = editor;
      _world = world;
      _spritePicker.HandleSelectedSprite = SpritePicker_HandleSelected;
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    public override string GetName()
    {
      return "World";
    }
        
    public void DrawArtifacts(Batcher batcher, Camera camera, Editor editor, GuiData gui)
    {
      if (SelectedLevel != -1)
      {
        batcher.DrawRectOutline(camera, _world.CurrentLevel.Bounds, editor.ColorSet.SpriteRegionActiveOutline);
      }
    }
    List<Point> _canFillTiles = new List<Point>();
    void SpritePicker_HandleSelected()
    {
      var input = Core.GetGlobalManager<Input.InputManager>();
      var rawMouse = Nez.Input.RawMousePosition.ToVector2().ToNumerics();

      if (SelectedSprite is Sprite sprite)
      {
        var min = sprite.Region.Location.ToVector2() / sprite.Texture.GetSize();
        var max = (sprite.Region.Location + sprite.Region.Size).ToVector2() / sprite.Texture.GetSize();
        var tilePos = rawMouse;
        tilePos.X = (int)(tilePos.X / sprite.TileSize.X) * sprite.TileSize.X;
        tilePos.Y = (int)(tilePos.Y / sprite.TileSize.Y) * sprite.TileSize.Y; 
       
        void PaintPreviewAt(System.Numerics.Vector2 screenPos)
        { 
          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
              screenPos - sprite.Region.GetHalfSize().ToNumerics() * _world.Scene.Camera.RawZoom, 
              screenPos - sprite.Region.GetHalfSize().ToNumerics() + sprite.Region.Size.ToVector2().ToNumerics() * _world.Scene.Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());
        }

        switch (PaintType)
        {
          case PaintType.Single: 
            PaintPreviewAt(tilePos);
            break;
          case PaintType.Fill:
            break;
          case PaintType.Rectangle:
            if (input.IsDrag && !input.IsDragFirst)
            {
              ImGui.GetForegroundDrawList().AddRectFilled(
                  input.MouseDragArea.Location.ToNumerics(), input.MouseDragArea.Max.ToNumerics(), 
                  _editor.ColorSet.SpriteRegionActiveFill.Add(new Color(1f, 1f, 1f, 0.3f)).ToImColor());
            }
            break;
        }

        var tilesToPaint = sprite.GetRectTiles();
        void PaintAtLayer(TileLayer tileLayer, Point tileInLayer)
        {
          var tileStart = tilesToPaint.First();
          if (tileStart == null) return;
          foreach (var tile in tilesToPaint)
          {
            var delta = tile.Coordinates - tileStart.Coordinates;
            if (PaintMode == PaintMode.Pen) tileLayer.ReplaceTile(tileInLayer + delta, new TileInstance(tile));
            else if (PaintMode == PaintMode.Eraser) tileLayer.RemoveTile(tileInLayer + delta);
          }
        }
        void PaintAt(TileLayer tileLayer, Vector2 worldPos)
        {
          var tileInLayer = tileLayer.GetTileCoordFromWorld(worldPos); 
          var tileStart = tilesToPaint.First();
          if (tileStart == null) return;
          foreach (var tile in tilesToPaint)
          {
            var delta = tile.Coordinates - tileStart.Coordinates;
            if (PaintMode == PaintMode.Pen) tileLayer.ReplaceTile(tileInLayer + delta, new TileInstance(tile));
            else if (PaintMode == PaintMode.Eraser) tileLayer.RemoveTile(tileInLayer + delta);
          }
        }
        if (_world.CurrentLevel != null && _world.CurrentLevel.CurrentLayer is TileLayer tileLayer && !input.IsImGuiBlocking)
        {
          var spriteCenter =  _world.Scene.Camera.MouseToWorldPoint() - sprite.Region.GetHalfSize();
          if (Nez.Input.LeftMouseButtonDown && PaintType == PaintType.Single)
          {
            PaintAt(tileLayer, spriteCenter);
          }
          if (PaintType == PaintType.Rectangle)
          {

            if (input.IsDragFirst) _mouseStart = _world.Scene.Camera.MouseToWorldPoint();
            if (input.IsDragLast) 
            {
              var rect = new RectangleF(); 
              rect.Location = _mouseStart;
              rect.Size = _world.Scene.Camera.MouseToWorldPoint() - _mouseStart;

              for (int x = 0; x < rect.Size.X; x+=sprite.Region.Size.X)
              {
                for (int y = 0; y < rect.Size.Y; y+=sprite.Region.Size.Y)
                {
                  PaintAtLayer(tileLayer, tileLayer.GetTileCoordFromWorld(rect.Location + new Vector2(x, y)));
                  
                }
              }
            }
          }
          if (PaintType == PaintType.Fill)
          {
            var tileInLayer = tileLayer.GetTileCoordFromWorld(spriteCenter); 
            var tileStart = tilesToPaint[tilesToPaint.Count/2];
            if (tileStart == null) return;
            _canFillTiles = _tileFiller.Update(tileLayer, tileInLayer, sprite.Region.Size/sprite.TileSize);
            if (Nez.Input.LeftMouseButtonPressed) 
            {
              void FloodFill(List<Point> fill)
              {
                foreach (var tile in _canFillTiles) 
                {
                  PaintAtLayer(tileLayer, tile);
                }
              }
              _tileFiller.Start(FloodFill);
            }
          }
        }
      }
      else if (SelectedSprite is Spritex spritex)
      {
        foreach (var part in spritex.Body)
        {
          var min = part.SourceSprite.Region.Location.ToVector2() / part.SourceSprite.Texture.GetSize();
          var max = (part.SourceSprite.Region.Location + part.SourceSprite.Region.Size).ToVector2() / part.SourceSprite.Texture.GetSize();
          var tilePos = rawMouse + part.LocalBounds.Location.ToNumerics();
          tilePos.X = (int)(tilePos.X / part.SourceSprite.TileSize.X) * part.SourceSprite.TileSize.X;
          tilePos.Y = (int)(tilePos.Y / part.SourceSprite.TileSize.Y) * part.SourceSprite.TileSize.Y; 

          ImGui.GetForegroundDrawList().AddImage(
              Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(part.SourceSprite.Texture),
              tilePos - spritex.EnclosingBounds.GetHalfSize().ToNumerics() * _world.Scene.Camera.RawZoom, 
              tilePos - spritex.EnclosingBounds.GetHalfSize().ToNumerics() + spritex.EnclosingBounds.Size.ToNumerics() * _world.Scene.Camera.RawZoom,
              min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());

        }

        if (_world.CurrentLevel != null && _world.CurrentLevel.CurrentLayer is TileLayer tilelayer)
        {
          if (Nez.Input.LeftMouseButtonDown && !input.IsImGuiBlocking)
          { 
            var tileApprox = _world.Scene.Camera.MouseToWorldPoint() - spritex.Bounds.Size/2f; 
            var tileInLayer = tilelayer.GetTileCoordFromWorld(tileApprox); 
            tilelayer.ReplaceTile(tileInLayer, new SpritexInstance(spritex));
          }
        }
      }
    }
    Vector2 _mouseStart = Vector2.Zero;
    TileFillFlooder _tileFiller = new TileFillFlooder();
    bool _isQueuedPaint = false;
    void SyncLevelGuis()
    {
      if (_levelGuis.Count() != _world.Levels.Count())
      {
        _levelGuis.Clear();
        foreach (var level in _world.Levels) 
        {
          _levelGuis.Add(new LevelGui(level, this));
          if (_world.CurrentLevel != null && _world.CurrentLevel.Name == level.Name) _levelGuis.Last().Selected = true;
        }
      }
    }

    bool _isOpenSheetHeaderPopup = false;
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      // The base window
      base.RenderImGui(renderer);

      if (SelectedLevel != -1)
      {
        _levelGuis[SelectedLevel].RenderImGui(renderer);
      }

      FocusFactor = _spritePicker.OpenSheet == null;
      _spritePicker.Draw();
      SelectedSprite = _spritePicker.SelectedSprite;
      if (_spritePicker.SelectedSprite != null && Nez.Input.RightMouseButtonReleased) CleanSelected();


      if (_isOpenSheetHeaderPopup)
      {
        _isOpenSheetHeaderPopup = false;
        ImGui.OpenPopup("sheet-header-options-popup");
      }
      if (ImGui.BeginPopupContextItem("sheet-header-options-popup"))
      {
        if (ImGui.MenuItem("Add Sheet"))
        {
        }
        ImGui.EndPopup();
      }
    }
    void CleanSelected()
    {
      _spritePicker.SelectedSprite = null;
    }
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{_world.Size.X} px");
      ImGui.LabelText("Height", $"{_world.Size.Y} px");
      ImGui.EndDisabled();


      // Another dock, levels panel
      var stack = new System.Numerics.Vector2(0, 0);
      var levelFlags = ImGuiTreeNodeFlags.None;
      if (_world.Levels.Count() > 0) levelFlags = ImGuiTreeNodeFlags.DefaultOpen;
      SyncLevelGuis();

      if (_spritePicker.Sheets.Count() != _world.SpriteSheets.Count())
      {
        _spritePicker.Sheets.Clear();
        foreach (var sheet in _world.SpriteSheets) 
        {
          var data = new SheetPickerData(sheet.Value);
          data.GridColor = renderer.Editor.ColorSet.SpriteRegionInactiveOutline.ToImColor();
          data.HoverTileFillColor = renderer.Editor.ColorSet.SpriteRegionActiveFill.Add(new Color(0.7f, 0.7f, 0.7f, 0.4f)).ToImColor();
          data.HoverTileBorderColor = renderer.Editor.ColorSet.SpriteRegionActiveOutline.ToImColor();
          _spritePicker.Sheets.Add(data);

        } 
      }

      // Levels listings
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.ObjectGroup + "   Levels", levelFlags))
      {
        var size = 200;
        stack.Y += size;
        ImGui.BeginChild("levels-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        for (int i = 0; i < _levelGuis.Count(); i++)
        {
          var level = _levelGuis[i];
          if (ImGui.Selectable($"{i+1}. {level.Name}", ref level.Selected, ImGuiSelectableFlags.AllowItemOverlap))
          {
            // Clear selection if not held with CTRL
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var levelGui in _levelGuis)
              {
                levelGui.Selected = false;
              }
            }
            SelectedLevel = i;
            level.Selected = true;
          }
          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(level.Name).X - 120, 0f));
          ImGui.SameLine();
          ImGui.PushID($"level-{level.Name}-id");
          var visibState = (!level._level.Enabled) ? IconFonts.FontAwesome5.EyeSlash : IconFonts.FontAwesome5.Eye;
          if (ImGui.SmallButton(visibState))
          {
            level._level.Enabled = !level._level.Enabled;
          }
          ImGui.SameLine();
          if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
          {
            _world.RemoveLevel(level._level);
            renderer.Editor.GetSubEntity<Selection>().End();
            SelectedLevel = -1;
          }
          ImGui.PopID();
          stack.Y += ImGui.GetItemRectSize().Y;
        }
        ImGui.EndChild();
      }
      SheetPickerData removeSheet = null;

      // Spritesheet listings
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.BorderAll + "   SpriteSheets", ImGuiTreeNodeFlags.DefaultOpen))
      {
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.IsWindowHovered()) _isOpenSheetHeaderPopup = true;
        var size = 140;
        stack.Y += size;
        ImGui.BeginChild("spritesheets-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        for (int i = 0; i < _spritePicker.Sheets.Count(); i++)
        {
          var sheet = _spritePicker.Sheets[i];
          
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.NoTreePushOnOpen 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 
          if (sheet.Selected)
          {
            flags |= ImGuiTreeNodeFlags.Selected;
          }
          var name = sheet.Sheet.Name;
          if (name.Count() > 20) name = name.Substring(0, 20) + "...";
          var sheetNode = ImGui.TreeNodeEx(sheet.Sheet.Name, flags); 

          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var sheetJ in _spritePicker.Sheets) sheetJ.Selected = false;
            }
            sheet.Selected = true;
          }

          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(sheet.Sheet.Name).X - 100 , 0f));
          ImGui.SameLine();
          if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
          {
            removeSheet = sheet;
          }

          if (sheetNode)
          {
            _spritePicker.SelectedSheet = sheet;
            // Draw preview spritesheet
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
        ImGui.EndChild();
      }
      if (removeSheet != null)
      {
        _spritePicker.Sheets.Remove(removeSheet);
        _world.SpriteSheets.Remove(removeSheet.Sheet.Name);
      }

    }

  }
}

