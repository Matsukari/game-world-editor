using Nez;
using Microsoft.Xna.Framework.Input;

namespace Raven.Sheet 
{
  public enum SelectionAxis { TopLeft, BottomLeft, Left, TopRight, BottomRight, Right, Top, Bottom, None }
  // <summary>
  // Edits the given bounds. Started by calling .Begin(). Automatically calls .End by itself 
  // </summary>
  public class Selection : EditorComponent
  {
    public int MouseButton = 0;
    public bool IsEditingPoint { get => SelAxis != SelectionAxis.None; }
    public SelectionAxis SelAxis = SelectionAxis.None;
    public RectangleF ContentBounds = new RectangleF();
    public RectangleF InitialBounds = new RectangleF();
    public object Capture = null;
    public int Radius = 5;
    public int SafeBuffer = 3;
    public float SelectedSelectionPointSizeFactor = 1.5f;
    public List<RectangleF> Points = new List<RectangleF>();
    public Selection() 
    { 
      RenderLayer = -1;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      int axis = -1, i = 0;

            // Draw the selection area
      batcher.DrawRect(ContentBounds, Editor.Settings.Colors.SelectionFill.ToColor());
      batcher.DrawRectOutline(camera, ContentBounds, Editor.Settings.Colors.SelectionOutline.ToColor(), 2);

      // Determine which resize point i being handled
      foreach (var point in Points)
      {
        var centerPoint = point;
        centerPoint.Size /= camera.RawZoom;
        centerPoint = centerPoint.GetCenterToStart();
        if (centerPoint.Contains(camera.MouseToWorldPoint())) 
        {
          axis = i;
        }
        batcher.DrawRect(centerPoint, Editor.Settings.Colors.SelectionPoint.ToColor());
        i++;
      }
      // Enlargen the resizing point currently on hover
      if (axis >= 0 && axis < (int)SelectionAxis.None) 
      {
        var point = Points[axis];
        var centerPoint = point;
        centerPoint.Size *= 1.3f;
        centerPoint.Size /= camera.RawZoom;
        centerPoint = centerPoint.GetCenterToStart();
        batcher.DrawRect(centerPoint, Editor.Settings.Colors.SelectionPoint.ToColor());
      }


      var selectionPoint = axis != -1 ? (SelectionAxis)axis : SelectionAxis.None;

      if (selectionPoint != SelectionAxis.None)
      {
        // Parent.SetMouseCursor(selectionPoint);
      }
      if (Nez.Input.LeftMouseButtonPressed && !Core.GetGlobalManager<Raven.Input.InputManager>().IsImGuiBlocking)
      {
        if (selectionPoint != SelectionAxis.None && !IsEditingPoint)
        {
          SelAxis = selectionPoint;
        }
        else if (!ContentBounds.Contains(camera.MouseToWorldPoint()))
        {
          End();
        }
      }
    }
    public void UpdatePoints()
    {
      Points.Clear();
      Points.Add(new RectangleF(ContentBounds.Left , ContentBounds.Top         , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Left , ContentBounds.Bottom      , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Left , ContentBounds.Center.Y    , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Right , ContentBounds.Top        , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Right , ContentBounds.Bottom     , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Right , ContentBounds.Center.Y   , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Center.X , ContentBounds.Top     , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(ContentBounds.Center.X , ContentBounds.Bottom  , Radius+SafeBuffer, Radius+SafeBuffer)); 
    }

    public void Snap(int w, int h)
    {
      ContentBounds.Width = MathF.Floor(ContentBounds.Width / w) * w;
      ContentBounds.Height = MathF.Floor(ContentBounds.Height / w) * w;
    }
    public void Begin(RectangleF area, object capture=null)
    {
      End();
      ContentBounds = area;
      InitialBounds = area;
      Capture = capture;
      Enabled = true;
      Core.GetGlobalManager<Raven.Input.InputManager>().IsDragFirst = true;
      Update();
    }
    public void End()
    {
      Capture = null;
      Enabled = false;
      SelAxis = SelectionAxis.None;
    }
    public bool HasBegun() => Enabled;

    RectangleF _selectionInitial = new RectangleF();
    bool _isDragInsideArea = false;
    public override void Update()
    {
      base.Update();
      UpdatePoints();

      var input = Core.GetGlobalManager<Raven.Input.InputManager>();

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
      }


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
