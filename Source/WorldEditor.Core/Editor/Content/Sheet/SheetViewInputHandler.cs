using Microsoft.Xna.Framework;
using Nez;


namespace Raven
{
  public class SheetViewInputHandler : EditorInterface, IInputHandler
  {
    readonly SheetView _view;
    Sheet _sheet { get => _view.Sheet; }
    List<int> _selectedTiles = new List<int>();
    Vector2 _initialMouse = Vector2.Zero;

    public event Action OnSelectionRightClick;
    public bool IsBlocked = false;
    
    public SheetViewInputHandler(SheetView view) => _view = view;

    public void RemoveSelection()
    {
      _selectedTiles.Clear(); 
      _initialMouse = Vector2.Zero;
      ContentData.SelectionList.Selections.Clear();
    }


    void IInputHandler.OnInputBlocked(Raven.InputManager input)
    {
      IsBlocked = true;
    }
    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      IsBlocked = false;
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
      if (input.IsDragFirst && Nez.Input.LeftMouseButtonDown) 
      {
        _initialMouse = Camera.MouseToWorldPoint();
        
 
      }
      else if (input.IsDrag && Nez.Input.LeftMouseButtonDown)
      {
        var mouseDragArea = new RectangleF();
        mouseDragArea.Location = _view.GetOffRegionInSheet(_initialMouse);
        mouseDragArea.Size = Camera.MouseToWorldPoint() - _initialMouse;
        mouseDragArea = mouseDragArea.AlwaysPositive();
        _selectedTiles = _view.Sheet.GetTiles(mouseDragArea);
        IsBlocked = true;
        var sprite = _sheet.CreateSprite(_selectedTiles.ToArray());
        Select(sprite);
      }
      else if (input.IsDragLast && Nez.Input.LeftMouseButtonReleased)
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
      }
      return ContentData.SelectionList.NotEmpty();
    }
    public void Select(IPropertied sel)
    {
      ContentData.SelectionList.Add(sel);
    }

  }
}

