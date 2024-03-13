using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetSelector : Editor.SubEntity
  {
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(HandleEditSprite);
    }
    void HandleEditSprite()
    {
      if (ImGui.GetIO().MouseClicked[1] && Editor.GetSubEntity<SheetView>().HasNoObstruction() && Gui.Selection != null) ImGui.OpenPopup("sprite-popup");
      if (Gui.Selection is Sprites.Sprite && ImGui.BeginPopupContextItem("sprite-popup"))
      {
        if (ImGui.BeginMenu(IconFonts.FontAwesome5.PlusSquare + " Convert to"))
        {
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Users + " Spritex")) 
          {
            ImGui.EndMenu();
            ImGui.EndPopup();
            ImGui.CloseCurrentPopup();
            ImGui.OpenPopup("spritex-name");
            return;
          }
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      }
      ImGuiViews.NamePopupModal(Editor, "spritex-name", ()=>
      {
        var spritex = Editor.SpriteSheet.CreateSpritex(ImGuiViews.InputName, Gui.Selection as Sprites.Sprite);
        Editor.SpriteSheet.Spritexes.TryAdd(spritex.Name, spritex);
        Editor.GetSubEntity<SpritexView>().Edit(spritex);
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
    void HandleSheetInputs()
    { 
      var sheetView = Editor.GetSubEntity<SheetView>();
      var input = Core.GetGlobalManager<Input.InputManager>();
      if (!Editor.GetSubEntity<SheetView>().Enabled || input.IsImGuiBlocking) return;

      if (input.IsDragFirst && _initialMouse == Vector2.Zero) 
      {
        _initialMouse = Scene.Camera.MouseToWorldPoint();
      }
      else if (input.IsDrag && _initialMouse != Vector2.Zero)
      {
        var mouseDragArea = new RectangleF();
        mouseDragArea.Location = sheetView.GetOffRegionInSheet(_initialMouse);
        mouseDragArea.Size = Scene.Camera.MouseToWorldPoint() - _initialMouse;
        _selectedTiles = Editor.SpriteSheet.GetTiles(mouseDragArea);
        if (_selectedTiles.Count > 1) 
        {
          var sprite = Editor.SpriteSheet.CreateSprite(_selectedTiles.ToArray());
          Select(sprite);
        }
      }
      else if (input.IsDragLast)
      {
        var count = _selectedTiles.Count;
        _selectedTiles.Clear();
        _initialMouse = Vector2.Zero;
        if (count > 1 || !sheetView.HasNoObstruction()) return;
        foreach (var sprite in Editor.SpriteSheet.Sprites)
        {
          if (sheetView.GetRegionInSheet(sprite.Value.Region.ToRectangleF()).Contains(Scene.Camera.MouseToWorldPoint())) 
            Select(sprite.Value);
        }
        if (sheetView.TileInMouse != null) 
        {
          var coord = Editor.SpriteSheet.GetTile(sheetView.TileInMouse);
          var tileInCoord = Editor.SpriteSheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Sprites.Tile(coord, Editor.SpriteSheet));
        }
        else if (Gui.Selection != null) 
        {
          Gui.Selection = null;
          Editor.Set(Editor.EditingState.Default);
        }
      }
    }

    public void Select(Shape shape)
    {
      Gui.ShapeSelection = shape;
    }
    public void Select(IPropertied sel)
    {
      if (Gui.ShapeSelection != null) return;
      Gui.Selection = sel;
      if (Gui.Selection is Sprites.Sprite sprite)
      {
      }
      else if (Gui.Selection is Sprites.Tile tile)
      {
      }
      else if (Gui.Selection is Sprites.Spritex spritex)
      {
      }
      else throw new TypeAccessException();
      Console.WriteLine($"Selected {Gui.Selection.GetType().Name}");
    }

  }
}

