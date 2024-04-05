using Nez;
using Microsoft.Xna.Framework.Input;

namespace Raven 
{
  public enum SelectionAxis { TopLeft, BottomLeft, Left, TopRight, BottomRight, Right, Top, Bottom, None }


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
      if (Nez.Input.LeftMouseButtonPressed) 
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
        centerPoint.Size *= 1.3f;
        centerPoint.Size /= camera.RawZoom;
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


  public class SelectionPoints 
  {
    public List<RectangleF> Points = new List<RectangleF>();
    public int Radius = 5;
    public int SafeBuffer = 3;

    public SelectionPoints()
    {
      for (int i = 0; i < 8; i++)
      {
        Points.Add(new RectangleF(0, 0, Radius, Radius));
      }
    }

    public void Update(RectangleF area)
    {
      Points.Clear();
      Points.Add(new RectangleF(area.Left , area.Top         , Radius, Radius));
      Points.Add(new RectangleF(area.Left , area.Bottom      , Radius, Radius));
      Points.Add(new RectangleF(area.Left , area.Center.Y    , Radius, Radius));
      Points.Add(new RectangleF(area.Right , area.Top        , Radius, Radius));
      Points.Add(new RectangleF(area.Right , area.Bottom     , Radius, Radius));
      Points.Add(new RectangleF(area.Right , area.Center.Y   , Radius, Radius));
      Points.Add(new RectangleF(area.Center.X , area.Top     , Radius, Radius));
      Points.Add(new RectangleF(area.Center.X , area.Bottom  , Radius, Radius)); 
    } 
  }
  // <summary>
  // Edits the given bounds. Started by calling .Begin(). Automatically calls .End by itself 
  // </summary>
  public class Selection : Component, IInputHandler 
  {
    public bool IsEditingPoint { get => SelAxis != SelectionAxis.None; }
    public SelectionPoints Points = new SelectionPoints();
    public SelectionAxis SelAxis = SelectionAxis.None;
    public RectangleF ContentBounds = new RectangleF();
    public RectangleF InitialBounds = new RectangleF();
    public float SelectedSelectionPointSizeFactor = 1.5f;
    public object Capture = null;
    bool _started = false;

    public event Action OnBegin;
        
    public void Snap(int w, int h)
    {
      ContentBounds.Width = MathF.Floor(ContentBounds.Width / w) * w;
      ContentBounds.Height = MathF.Floor(ContentBounds.Height / w) * w;
    }
    public void Begin(RectangleF area, object capture=null)
    {
      Core.GetGlobalManager<InputManager>().IsDragFirst = true;
      End();
      ContentBounds = area;
      InitialBounds = area;
      Capture = capture;
      Points.Update(ContentBounds);
      _started = true;

      if (OnBegin != null)
        OnBegin();
    }
    public void End()
    {
      Capture = null;
      SelAxis = SelectionAxis.None;
      _started = false;
    }
    public bool HasBegun() => _started;

    RectangleF _selectionInitial = new RectangleF();
    bool _isDragInsideArea = false;

    bool IInputHandler.OnHandleInput(InputManager input)   
    {
      // The distance between the movement of the mouse
      var _bounds = ContentBounds;
      var mouse = Nez.Input.RawMousePosition.ToVector2();
      var delta = mouse - input.MouseDragStart;
      delta /= Entity.Scene.Camera.RawZoom;

      // start whatever selection
      if (input.IsDragFirst)
      {
        _selectionInitial = ContentBounds;

        // mouse is also inside the selection area; can insead be moved
        if (_bounds.Width != 0 && _bounds.Contains(Entity.Scene.Camera.MouseToWorldPoint()))
        {
          _isDragInsideArea = true;
        }
      } 
      else if (input.IsDragLast) 
      {
        _isDragInsideArea = false;
        Mouse.SetCursor(MouseCursor.Arrow);
        if (IsEditingPoint)
        {
          SelAxis = SelectionAxis.None;
        }
      }

      // draggin point
      if (IsEditingPoint)
      {
        // SetMouseCursor(SelAxis);
        switch (SelAxis)
        {
          case SelectionAxis.TopLeft:
            _bounds.X = _selectionInitial.X + delta.X;
            _bounds.Y = _selectionInitial.Y + delta.Y;
            _bounds.Size = _selectionInitial.Size - delta;
            break;

          case SelectionAxis.TopRight:
            _bounds.Y = _selectionInitial.Y + delta.Y;
            _bounds.Height = _selectionInitial.Height - delta.Y;
            _bounds.Width = _selectionInitial.Width + delta.X;
            break;

          case SelectionAxis.BottomLeft:
            _bounds.X = _selectionInitial.X + delta.X;
            _bounds.Width = _selectionInitial.Width - delta.X;
            _bounds.Height = _selectionInitial.Height + delta.Y; 
            break;

          case SelectionAxis.BottomRight: 
            _bounds.Size = _selectionInitial.Size + delta; 
            break;

          case SelectionAxis.Right: 
            _bounds.Width = _selectionInitial.Width + delta.X; 
            break;

          case SelectionAxis.Left: 
            _bounds.X = _selectionInitial.X + delta.X; 
            _bounds.Width = _selectionInitial.Width - delta.X; 
            break;

          case SelectionAxis.Top: 
            _bounds.Y = _selectionInitial.Y + delta.Y; 
            _bounds.Height = _selectionInitial.Height - delta.Y; 
            break;

          case SelectionAxis.Bottom: 
            _bounds.Height = _selectionInitial.Height + delta.Y; 
            break;
        }
        ContentBounds = _bounds;
      }
      // move selection
      else if (input.IsDrag && _isDragInsideArea && Nez.Input.LeftMouseButtonDown && !input.IsImGuiBlocking)
      {
        _bounds.Location = _selectionInitial.Location + delta;
        ContentBounds = _bounds;
        return true;
      }

      return IsEditingPoint;
    }
    public void SetMouseCursor(SelectionAxis axis)
    {
      switch (axis)
      {
        case SelectionAxis.TopLeft:
          Mouse.SetCursor(MouseCursor.SizeNWSE);
          break;

        case SelectionAxis.TopRight:
          Mouse.SetCursor(MouseCursor.SizeNESW);
          break;

        case SelectionAxis.BottomLeft:
          Mouse.SetCursor(MouseCursor.SizeNESW);
          break;

        case SelectionAxis.BottomRight: 
          Mouse.SetCursor(MouseCursor.SizeNWSE);
          break;

        case SelectionAxis.Right: 
          Mouse.SetCursor(MouseCursor.SizeWE);
          break;

        case SelectionAxis.Left: 
          Mouse.SetCursor(MouseCursor.SizeWE);
          break;

        case SelectionAxis.Top: 
          Mouse.SetCursor(MouseCursor.SizeNS);
          break;

        case SelectionAxis.Bottom: 
          Mouse.SetCursor(MouseCursor.SizeNS);
          break;
      }
  }  
}


}
