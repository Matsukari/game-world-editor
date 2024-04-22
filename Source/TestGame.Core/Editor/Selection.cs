using Nez;
using Microsoft.Xna.Framework.Input;

namespace Raven 
{

  public enum SelectionAxis { TopLeft, BottomLeft, Left, TopRight, BottomRight, Right, Top, Bottom, None }

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
      ContentBounds = new RectangleF();
      _started = false;
      _isDragInsideArea = false;
    }
    public bool HasBegun() => _started;

    RectangleF _selectionInitial = new RectangleF();
    bool _isDragInsideArea = false;

    int IInputHandler.Priority() => 9;

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
        Console.WriteLine("Inside draggin");
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
