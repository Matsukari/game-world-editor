using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetSelector : Editor.SheetEntity
  {
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(HandleEditSprite);
    }
    Sprites.Spritex _spritexOnName = null;
    void HandleEditSprite()
    {
      if (!Enabled) return;
      
      var spritexView = Editor.GetSubEntity<SpritexView>();

      // right-click; open options
      if (Nez.Input.RightMouseButtonPressed 
          && Editor.GetSubEntity<SheetView>().HasNoObstruction() 
          && Gui.Selection != null) ImGui.OpenPopup("sprite-popup");

      void OpenPopup(string name)
      {
        ImGui.EndPopup();
        ImGui.CloseCurrentPopup();
        ImGui.OpenPopup(name);
      }

      // something is selected; render options
      if (Gui.Selection is Sprites.Sprite sprite && ImGui.BeginPopupContextItem("sprite-popup"))
      {
        // convert to new spritex
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + " Convert to Spritex"))
        {
          OpenPopup("spritex-name");
          return;
        }
        // add to exisiting spritex; select by list
        if (Sheet.Spritexes.Count() > 0 && ImGui.BeginMenu(IconFonts.FontAwesome5.UserPlus + " Add to Spritex"))
        {
          foreach (var spritex in Sheet.Spritexes)
          {
            if (ImGui.MenuItem(IconFonts.FontAwesome5.Users + " " + spritex.Key)) 
            {
              ImGui.EndMenu();
              OpenPopup("spritex-part-name");
              _spritexOnName = spritex.Value;
              return;
            }
          }
          ImGui.EndMenu();
        }
        // Has opened and made operation to last spritex' part
        if (spritexView.LastSprite != null)
        {
          if (ImGui.MenuItem(IconFonts.FontAwesome5.UserPlus + " Add to last Spritex"))
          {
            _spritexOnName = spritexView.LastSprite.Spritex;
            OpenPopup("spritex-part-name");
            return;
          }
          if (spritexView.LastSprite.ChangePart != null && ImGui.MenuItem(IconFonts.FontAwesome5.UserPlus + " Change last Spritex part"))
          {
            ImGui.EndPopup();
            ImGui.CloseCurrentPopup();
            spritexView.LastSprite.Spritex.Parts.Data[spritexView.LastSprite.ChangePart.Name] = new Sprites.SourcedSprite(spritexView.LastSprite.Spritex, sprite);
            spritexView.LastSprite.Spritex.Parts.Data[spritexView.LastSprite.ChangePart.Name].Name = spritexView.LastSprite.ChangePart.Name;
            spritexView.Edit(spritexView.LastSprite.Spritex);
            return;
          }
        }
        ImGui.EndPopup();
      }
      ImGuiViews.NamePopupModal(Editor, "spritex-name", ()=>
      {
        var spritex = Sheet.CreateSpritex(ImGuiViews.InputName, Gui.Selection as Sprites.Sprite);
        Sheet.Spritexes.TryAdd(spritex.Name, spritex);
        Editor.GetSubEntity<SpritexView>().Edit(spritex);
      });
      ImGuiViews.NamePopupModal(Editor, "spritex-part-name", ()=>
      {
        _spritexOnName.AddSprite(ImGuiViews.InputName, new Sprites.SourcedSprite(sprite: Gui.Selection as Sprites.Sprite));
      });
      
    } 
    public override void Update()
    {
      base.Update();
      var sheetView = Editor.GetSubEntity<SheetView>();
      if (Editor.GetSubEntity<SheetView>().Enabled) 
      {
        HandleSheetInputs();
      }
    }
    List<int> _selectedTiles = new List<int>();
    Vector2 _initialMouse = Vector2.Zero;
    public void RemoveSelection()
    {
      _selectedTiles.Clear(); 
      _initialMouse = Vector2.Zero;
      Gui.Selection = null;
    }
    void HandleSheetInputs()
    { 
      var sheetView = Editor.GetSubEntity<SheetView>();
      var input = Core.GetGlobalManager<Input.InputManager>();
      if (!Editor.GetSubEntity<SheetView>().Enabled || input.IsImGuiBlocking || Editor.EditState != Editor.EditingState.Default) return;

      // Foundation for multiple tile selection
      if (input.IsDragFirst && _initialMouse == Vector2.Zero) 
      {
        _initialMouse = Scene.Camera.MouseToWorldPoint();
      }
      else if (input.IsDrag && _initialMouse != Vector2.Zero)
      {
        var mouseDragArea = new RectangleF();
        mouseDragArea.Location = sheetView.GetOffRegionInSheet(_initialMouse);
        mouseDragArea.Size = Scene.Camera.MouseToWorldPoint() - _initialMouse;
        mouseDragArea = mouseDragArea.AlwaysPositive();
        _selectedTiles = Sheet.GetTiles(mouseDragArea);
        if (_selectedTiles.Count > 1) 
        {
          var sprite = Sheet.CreateSprite(_selectedTiles.ToArray());
          Select(sprite);
        }
      }
      else if (input.IsDragLast)
      {
        var count = _selectedTiles.Count;
        _selectedTiles.Clear();
        _initialMouse = Vector2.Zero;
        if (count > 1 || !sheetView.HasNoObstruction()) return;

        // Selects multiple tiles
        foreach (var sprite in Sheet.Sprites)
        {
          if (sheetView.GetRegionInSheet(sprite.Value.Region.ToRectangleF()).Contains(Scene.Camera.MouseToWorldPoint())) 
            Select(sprite.Value);
        }
        // Selects single tile
        if (sheetView.TileInMouse != null) 
        {
          var coord = Sheet.GetTile(sheetView.TileInMouse);
          var tileInCoord = Sheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Sprites.Tile(coord, Sheet));
        }
      }
    }
    public void Select(IPropertied sel)
    {
      if (Gui.ShapeSelection != null) return;
      Gui.Selection = sel;
      Console.WriteLine($"Selected {Gui.Selection.GetType().Name}");
    }

  }
}

