using Nez;

namespace Raven
{
	 public class SelectionRenderer : RenderableComponent, IInputHandler 
  {
    readonly Selection _selection;
    readonly EditorColors _colors;
    bool _selected = false;

    public SelectionRenderer(Selection selection, EditorColors colors)
    {
      _selection = selection;
      _colors = colors;
    }

    bool IInputHandler.OnHandleInput(InputManager input)
    {
      if (Nez.Input.LeftMouseButtonPressed && _selection.HasBegun()) 
        _selected = true;

      return _selected;
    }
    public override bool IsVisibleFromCamera(Camera camera) => true;      

    public override void Render(Batcher batcher, Camera camera)
    {
      _selection.Points.Update(_selection.ContentBounds);

      int axis = -1, i = 0;

      // Draw the selection area
      batcher.DrawRect(_selection.ContentBounds, _colors.SelectionFill.ToColor());
      batcher.DrawRectOutline(camera, _selection.ContentBounds, _colors.SelectionOutline.ToColor(), 2);

      // Determine which resize point i being handled
      foreach (var point in _selection.Points.Points)
      {
        var centerPoint = point;
        if (camera.RawZoom < 1) centerPoint.Size /= camera.RawZoom;
        centerPoint = centerPoint.GetCenterToStart();
        if (centerPoint.Contains(camera.MouseToWorldPoint())) 
        {
          axis = i;
        }
        batcher.DrawRect(centerPoint, _colors.SelectionPoint.ToColor());
        i++;
      }
      // Enlargen the resizing point currently on hover
      if (axis >= 0 && axis < (int)SelectionAxis.None) 
      {
        var point = _selection.Points.Points[axis];
        var centerPoint = point;
        centerPoint.Size *= 1.4f;
        if (camera.RawZoom < 1) centerPoint.Size /= camera.RawZoom;
        centerPoint = centerPoint.GetCenterToStart();
        batcher.DrawRect(centerPoint, _colors.SelectionPoint.ToColor());
      }


      var selectionPoint = axis != -1 ? (SelectionAxis)axis : SelectionAxis.None;

      if (selectionPoint != SelectionAxis.None)
      {
        // Parent.SetMouseCursor(selectionPoint);
      }
      if (_selected)
      {
        if (selectionPoint != SelectionAxis.None && !_selection.IsEditingPoint)
        {
          _selection.SelAxis = selectionPoint;
        }
        else if (!_selection.ContentBounds.Contains(camera.MouseToWorldPoint()))
        {
          _selection.End();
        }
        _selected = false;
      }
    }

  }
}
