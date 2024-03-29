using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  public class SheetSelector : EditorComponent, IImGuiRenderable
  {
    Sheet _sheet;
    Sprites.Spritex _spritexOnName = null;
    public override void OnContent()
    {
      if (RestrictTo<Sheet>())
      {
        _sheet = Content as Sheet;
      }
    } 
    public void Render(Editor editor)
    {
      var spritexView = Editor.GetEditorComponent<SpritexView>();

      // right-click; open options
      if (Nez.Input.RightMouseButtonPressed && ContentData.Selection != null) 
      {
        ImGui.OpenPopup("sprite-popup");
      }

      void OpenPopup(string name)
      {
        ImGui.EndPopup();
        ImGui.CloseCurrentPopup();
        ImGui.OpenPopup(name);
      }
     
      // something is selected; render options
      if (ContentData.Selection is Sprites.Sprite sprite && ImGui.BeginPopup("sprite-popup"))
      {
        // convert to new spritex
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + " Convert to Spritex"))
        {
          Editor.NameModal.Open((name)=>
          {
              var spritex = _sheet.CreateSpritex(name, ContentData.Selection as Sprites.Sprite);
              _sheet.Spritexes.AddIfNotPresent(spritex);
              Editor.GetEditorComponent<SpritexView>().Edit(spritex);
          });
        }
        // add to exisiting spritex; select by list
        if (_sheet.Spritexes.Count() > 0 && ImGui.BeginMenu(IconFonts.FontAwesome5.UserPlus + " Add to Spritex"))
        {
          foreach (var spritex in _sheet.Spritexes)
          {
            if (ImGui.MenuItem(IconFonts.FontAwesome5.Users + " " + spritex.Name)) 
            {
              _spritexOnName = spritex;
              Editor.NameModal.Open((name)=>
              {
                _spritexOnName.AddSprite(name, new Sprites.SourcedSprite(sprite: ContentData.Selection as Sprites.Sprite));
              });
            }
          }
          ImGui.EndMenu();
        }
        // // Has opened and made operation to last spritex' part
        // if (spritexView.LastSprite != null)
        // {
        //   if (ImGui.MenuItem(IconFonts.FontAwesome5.UserPlus + " Add to last Spritex"))
        //   {
        //     _spritexOnName = spritexView.LastSprite.Spritex;
        //     OpenPopup("spritex-part-name");
        //     return;
        //   }
        //   if (spritexView.LastSprite.ChangePart != null && ImGui.MenuItem(IconFonts.FontAwesome5.UserPlus + " Change last Spritex part"))
        //   {
        //     ImGui.EndPopup();
        //     ImGui.CloseCurrentPopup();
        //     spritexView.LastSprite.Spritex.Parts.Data[spritexView.LastSprite.ChangePart.Name] = new Sprites.SourcedSprite(spritexView.LastSprite.Spritex, sprite);
        //     spritexView.LastSprite.Spritex.Parts.Data[spritexView.LastSprite.ChangePart.Name].Name = spritexView.LastSprite.ChangePart.Name;
        //     spritexView.Edit(spritexView.LastSprite.Spritex);
        //     return;
        //   }
        // }
        ImGui.EndPopup();
      }


    } 
    public override void Update()
    {
      base.Update();
      HandleSheetInputs();
    }
    List<int> _selectedTiles = new List<int>();
    Vector2 _initialMouse = Vector2.Zero;
    public void RemoveSelection()
    {
      _selectedTiles.Clear(); 
      _initialMouse = Vector2.Zero;
      ContentData.Selection = null;
    }
    void HandleSheetInputs()
    { 
      var sheetView = Editor.GetEditorComponent<SheetView>();
      var input = Core.GetGlobalManager<Input.InputManager>();
      if (input.IsImGuiBlocking) return;

      // Foundation for multiple tile selection
      if (input.IsDragFirst && _initialMouse == Vector2.Zero && Nez.Input.LeftMouseButtonDown) 
      {
        _initialMouse = Entity.Scene.Camera.MouseToWorldPoint();
      }
      else if (input.IsDrag && _initialMouse != Vector2.Zero)
      {
        var mouseDragArea = new RectangleF();
        mouseDragArea.Location = sheetView.GetOffRegionInSheet(_initialMouse);
        mouseDragArea.Size = Entity.Scene.Camera.MouseToWorldPoint() - _initialMouse;
        mouseDragArea = mouseDragArea.AlwaysPositive();
        _selectedTiles = _sheet.GetTiles(mouseDragArea);
        if (_selectedTiles.Count > 1) 
        {
          var sprite = _sheet.CreateSprite(_selectedTiles.ToArray());
          Select(sprite);
        }
      }
      else if (input.IsDragLast)
      {
        var count = _selectedTiles.Count;
        _selectedTiles.Clear();
        _initialMouse = Vector2.Zero;
        if (count > 1) return;

        // Selects single tile
        if (sheetView.TileInMouse != null) 
        {
          var coord = _sheet.GetTile(sheetView.TileInMouse);
          var tileInCoord = _sheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Sprites.Tile(coord, _sheet));
        }
      }
    }
    public void Select(IPropertied sel)
    {
      if (ContentData.ShapeSelection != null || Editor.GetEditorComponent<Selection>().HasBegun()) return;
      ContentData.Selection = sel;
      Console.WriteLine($"Selected {ContentData.Selection.GetType().Name}");
    }

  }
}

