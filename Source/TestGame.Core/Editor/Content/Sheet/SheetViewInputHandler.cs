using Microsoft.Xna.Framework;
using Nez;

namespace Raven
{
  public class SheetViewInputHandler : EditorInterface, IInputHandler
  {
    Sheet _sheet { get => _view.Sheet; }
    readonly SheetView _view;

    public event Action OnSelected;
   
    // SpriteScene _spriteSceneOnName = null;

    public void Render(Editor editor)
    {
      // right-click; open options
      if (Nez.Input.RightMouseButtonPressed && ContentData.SelectionList.Selections != null) 
      {
        OnSelected();
        // ImGui.OpenPopup("sprite-popup");
      }

      // void OpenPopup(string name)
      // {
      //   ImGui.EndPopup();
      //   ImGui.CloseCurrentPopup();
      //   ImGui.OpenPopup(name);
      // }
     
      // something is selected; render options
      // if (ContentData.Selection is Sprites.Sprite sprite && ImGui.BeginPopup("sprite-popup"))
      // {
      //   // convert to new spriteScene
      //   if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + " Convert to SpriteScene"))
      //   {
      //     Editor.NameModal.Open((name)=>
      //     {
      //         var spriteScene = _sheet.CreateSpriteScene(name, ContentData.Selection as Sprites.Sprite);
      //         _sheet.SpriteScenees.AddIfNotPresent(spriteScene);
      //         Editor.GetEditorComponent<SpriteSceneView>().Edit(spriteScene);
      //     });
      //   }
      //   // add to exisiting spriteScene; select by list
      //   if (_sheet.SpriteScenees.Count() > 0 && ImGui.BeginMenu(IconFonts.FontAwesome5.UserPlus + " Add to SpriteScene"))
      //   {
      //     foreach (var spriteScene in _sheet.SpriteScenees)
      //     {
      //       if (ImGui.MenuItem(IconFonts.FontAwesome5.Users + " " + spriteScene.Name)) 
      //       {
      //         _spriteSceneOnName = spriteScene;
      //         Editor.NameModal.Open((name)=>
      //         {
      //           _spriteSceneOnName.AddSprite(name, new Sprites.SourcedSprite(sprite: ContentData.Selection as Sprites.Sprite));
      //         });
      //       }
      //     }
      //     ImGui.EndMenu();
      //   }
      //   ImGui.EndPopup();
      // }


    } 
    List<int> _selectedTiles = new List<int>();
    Vector2 _initialMouse = Vector2.Zero;
    public void RemoveSelection()
    {
      _selectedTiles.Clear(); 
      _initialMouse = Vector2.Zero;
      ContentData.SelectionList.Selections.Clear();
    }
    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      // Foundation for multiple tile selection
      if (input.IsDragFirst && _initialMouse == Vector2.Zero && Nez.Input.LeftMouseButtonDown) 
      {
        _initialMouse = Camera.MouseToWorldPoint();
        return true;
      }
      else if (input.IsDrag && _initialMouse != Vector2.Zero)
      {
        var mouseDragArea = new RectangleF();
        mouseDragArea.Location = _view.GetOffRegionInSheet(_initialMouse);
        mouseDragArea.Size = Camera.MouseToWorldPoint() - _initialMouse;
        mouseDragArea = mouseDragArea.AlwaysPositive();
        _selectedTiles = _view.Sheet.GetTiles(mouseDragArea);
        if (_selectedTiles.Count > 1) 
        {
          var sprite = _sheet.CreateSprite(_selectedTiles.ToArray());
          Select(sprite);
        }
        return true;
      }
      else if (input.IsDragLast)
      {
        var count = _selectedTiles.Count;
        _selectedTiles.Clear();
        _initialMouse = Vector2.Zero;
        if (count > 1) return true;

        // Selects single tile
        if (!_view.TileInMouse.IsEmpty) 
        {
          var coord = _sheet.GetTile(_view.TileInMouse);
          var tileInCoord = _sheet.CustomTileExists(coord.X, coord.Y);
          if (tileInCoord != null) Select(tileInCoord);
          else Select(new Tile(coord, _sheet));
        }
        return true;
      }
      return false;
    }
    public void Select(IPropertied sel)
    {
      ContentData.SelectionList.Add(sel);
    }

  }
}

