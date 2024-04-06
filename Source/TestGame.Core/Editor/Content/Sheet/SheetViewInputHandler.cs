using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven
{
  public class SheetViewPopup : EditorInterface, IImGuiRenderable
  {
    Sheet _sheet { get => Content as Sheet; }
    SpriteScene _spriteSceneOnName;
    bool _isOpenSpriteOptions = false;

    public event Action<SpriteScene> OnConvertToScene;

    public void OpenSpriteOptions() => _isOpenSpriteOptions = true;

    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      if (_isOpenSpriteOptions)
      {
        _isOpenSpriteOptions = false;
        ImGui.OpenPopup("sprite-popup");
      }

      // something is selected; render options
      if (ContentData.SelectionList.NotEmpty() && ContentData.SelectionList.Last() is Sprite sprite && ImGui.BeginPopup("sprite-popup"))
      {
        // convert to new spriteScene
        if (ImGui.MenuItem(IconFonts.FontAwesome5.PlusSquare + " Convert to SpriteScene"))
        {
          imgui.NameModal.Open((name)=>
          {
              var spriteScene = _sheet.CreateSpriteScene(name, ContentData.SelectionList.Last() as Sprite);
              _sheet.SpriteScenees.AddIfNotPresent(spriteScene);
              if (OnConvertToScene != null) OnConvertToScene(spriteScene);
          });
        }
        // add to exisiting spriteScene; select by list
        if (_sheet.SpriteScenees.Count() > 0 && ImGui.BeginMenu(IconFonts.FontAwesome5.UserPlus + " Add to SpriteScene"))
        {
          foreach (var spriteScene in _sheet.SpriteScenees)
          {
            if (ImGui.MenuItem(IconFonts.FontAwesome5.Users + " " + spriteScene.Name)) 
            {
              _spriteSceneOnName = spriteScene;
              imgui.NameModal.Open((name)=>
              {
                _spriteSceneOnName.AddSprite(name, new SourcedSprite(sprite as Sprite));
              });
            }
          }
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      } 
    }
  }

  public class SheetViewInputHandler : EditorInterface, IInputHandler
  {
    readonly SheetView _view;
    Sheet _sheet { get => _view.Sheet; }
    List<int> _selectedTiles = new List<int>();
    Vector2 _initialMouse = Vector2.Zero;

    public event Action OnSelectionRightClick;
    
    public SheetViewInputHandler(SheetView view) => _view = view;

    public void RemoveSelection()
    {
      _selectedTiles.Clear(); 
      _initialMouse = Vector2.Zero;
      ContentData.SelectionList.Selections.Clear();
    }
    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      if (_view.ImGuiHandler is SheetViewImGui imgui && imgui.SceneView.IsEditing)
      {
        return imgui.SceneView.HandleInput(input);
      }

      // right-click; open options
      if (Nez.Input.RightMouseButtonPressed && ContentData.SelectionList.Selections.Count > 0 && OnSelectionRightClick != null) 
      {
        OnSelectionRightClick();
        return true;
      }

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

