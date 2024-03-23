using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;
using Raven.Sheet.Sprites;

namespace Raven.Sheet
{
  // <summary>
  // Expands as the levels are painted on
  // </summary>
  public class WorldGui : Propertied
  {
    internal World _world;
    public object SelectedSprite;
    SpritePicker _spritePicker = new SpritePicker();
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
    public override string Name { get => _world.Name; set => _world.Name = value; }
        
    public WorldGui(World world) 
    {
      _world = world;
      _spritePicker.HandleSelectedSprite = SpritePicker_HandleSelected;
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    public void DrawArtifacts(Batcher batcher, Camera camera, Editor editor, GuiData gui)
    {
      if (SelectedLevel != -1)
      {
        batcher.DrawRectOutline(camera, _world.CurrentLevel.Bounds, editor.ColorSet.SpriteRegionActiveOutline);
      }
    }
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
        
        ImGui.GetForegroundDrawList().AddImage(
            Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture),
            tilePos - sprite.Region.GetHalfSize().ToNumerics() * _world.Scene.Camera.RawZoom, 
            tilePos - sprite.Region.GetHalfSize().ToNumerics() + sprite.Region.Size.ToVector2().ToNumerics() * _world.Scene.Camera.RawZoom,
            min.ToNumerics(), max.ToNumerics(), new Color(0.8f, 0.8f, 1f, 0.5f).ToImColor());

        if (_world.CurrentLevel != null && _world.CurrentLevel.CurrentLayer is TileLayer tilelayer)
        {
          if (Nez.Input.LeftMouseButtonDown && !input.IsImGuiBlocking)
          {
            var tileApprox = _world.Scene.Camera.MouseToWorldPoint() - sprite.Region.GetHalfSize(); 
            var tileInLayer = tilelayer.GetTileCoordFromWorld(tileApprox); 
            var tileStart = sprite.GetRectTiles().First() ;
            if (tileStart == null) return;
            foreach (var tile in sprite.GetRectTiles())
            {
              var delta = tile.Coordinates - tileStart.Coordinates;
              tilelayer.ReplaceTile(tileInLayer + delta, new TileInstance(tile));
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
    void SyncLevelGuis()
    {
      if (_levelGuis.Count() != _world.Levels.Count())
      {
        _levelGuis.Clear();
        foreach (var level in _world.Levels) 
        {
          _levelGuis.Add(new LevelGui(level));
          if (_world.CurrentLevel != null && _world.CurrentLevel.Name == level.Name) _levelGuis.Last().Selected = true;
        }
      }
    }
    public override void RenderImGui(PropertiesRenderer renderer)
    {
      // The base window
      base.RenderImGui(renderer);

      if (SelectedLevel != -1)
      {
        _levelGuis[SelectedLevel].RenderImGui(renderer);
      }

      // Another dock, levels panel
      ImGui.Begin(IconFonts.FontAwesome5.ObjectGroup + " World Lister");
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
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.ObjectGroup + "  Levels", levelFlags))
      {
        var size = 100;
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
          ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(level.Name).X - 150, 0f));
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

      // Spritesheet listings
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.Th + "  SpriteSheets", ImGuiTreeNodeFlags.DefaultOpen))
      {
        var size = 100;
        stack.Y += size;
        ImGui.BeginChild("spritesheets-content");
        foreach (var sheet in _spritePicker.Sheets)
        {
          if (ImGui.MenuItem(sheet.Sheet.Name)) 
          {
            _spritePicker.SelectedSheet = sheet;
          }
        }
        ImGui.EndChild();

        // Draw preview spritesheet
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
      _spritePicker.Draw();
      SelectedSprite = _spritePicker.SelectedSprite;
      if (_spritePicker.SelectedSprite != null && Nez.Input.RightMouseButtonReleased) CleanSelected();
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
    }

  }
}

