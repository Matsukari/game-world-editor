using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;

namespace Raven.Sheet 
{
  public enum SelectionAxis
  {
    TopLeft,
   BottomLeft,
    Left,
    TopRight,
    BottomRight,
    Right,
    Top,
    Bottom,
    None,
  }
  public class Selection : Editor.SubEntity 
  {
    public int MouseButton = 0;
    public bool IsEditingPoint { get => SelAxis != SelectionAxis.None; }
    public SelectionAxis SelAxis = SelectionAxis.None;
    public Renderable Ren { get; private set; }
    public RectangleF Bounds = new RectangleF();
    public RectangleF InitialBounds = new RectangleF();
    public object Capture = null;
    public Selection() 
    { 
    }
    public override void OnAddedToScene()
    {
      base.OnAddedToScene();
      Ren = AddComponent(new Renderable());
      Ren.RenderLayer = -1;
      End();
    }
        
    public class Renderable : Editor.SubEntity.RenderableComponent<Selection>
    {
      public int Radius = 4;
      public int SafeBuffer = 3;
      public float SelectedSelectionPointSizeFactor = 1.5f;
      public List<RectangleF> Points = new List<RectangleF>();
      public override void Render(Batcher batcher, Camera camera)
      {
        int axis = -1, i = 0;

        // Determine which resize point i being handled
        foreach (var point in Points)
        {
          if (point.GetCenterToStart().Contains(camera.MouseToWorldPoint())) 
          {
            axis = i;
          }
          batcher.DrawCircle(point.Location, Radius, Editor.ColorSet.SelectionPoint);
          i++;
        }
        // Enlargen the resizing point currently on hover
        LargenSelectionAxis(axis, Editor.ColorSet.SelectionPoint, batcher, camera);

        // Draw the selection area
        batcher.DrawRect(Parent.Bounds, Editor.ColorSet.SelectionFill);
        batcher.DrawRectOutline(Parent.Bounds, Editor.ColorSet.SelectionOutline);

        var selectionPoint = axis != -1 ? (SelectionAxis)axis : SelectionAxis.None;
        if (Nez.Input.LeftMouseButtonPressed)
        {
          if (selectionPoint != SelectionAxis.None && !Parent.IsEditingPoint)
          {
            Parent.SelAxis = selectionPoint;
          }
          else if (Parent._hasInteraction && !Parent.Bounds.Contains(camera.MouseToWorldPoint()))
          {
            Parent.End();
          }
        }
      }
      public void UpdatePoints()
      {
        Points.Clear();
        Points.Add(new RectangleF(Parent.Bounds.Left , Parent.Bounds.Top         , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Left , Parent.Bounds.Bottom      , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Left , Parent.Bounds.Center.Y    , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Right , Parent.Bounds.Top        , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Right , Parent.Bounds.Bottom     , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Right , Parent.Bounds.Center.Y   , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Center.X , Parent.Bounds.Top     , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Parent.Bounds.Center.X , Parent.Bounds.Bottom  , Radius+SafeBuffer, Radius+SafeBuffer)); 
      }
      void LargenSelectionAxis(int point, Color color, Batcher batcher, Camera camera)
      {
        if (point >= 0 && point < (int)SelectionAxis.None) 
        { 
          Gui.primitiveBatch.Begin(projection: camera.ProjectionMatrix, view: camera.TransformMatrix);
          Gui.primitiveBatch.DrawCircle(Points[point].Location, Radius*SelectedSelectionPointSizeFactor, color);
          Gui.primitiveBatch.End();
        }
      }
    }
    public void Snap(int w, int h)
    {
      Bounds.Width = MathF.Floor(Bounds.Width / w) * w;
      Bounds.Height = MathF.Floor(Bounds.Height / w) * w;
    }
    public void Begin(RectangleF area, object capture=null)
    {
      End();
      Bounds = area;
      InitialBounds = area;
      Capture = capture;
      Enabled = true;
      Editor.Set(Editor.EditingState.SelectedSprite);
      Core.GetGlobalManager<Raven.Input.InputManager>().IsDragFirst = true;
      Update();
    }
    public void End()
    {
      Capture = null;
      Enabled = false;
      SelAxis = SelectionAxis.None;
      Editor.Set(Editor.EditingState.Default);
    }
    public bool HasBegun() => Enabled;

    RectangleF _selectionInitial = new RectangleF();
    bool _isDragInsideArea = false;
    bool _hasInteraction = false;
    public override void Update()
    {
      base.Update();
      Ren.UpdatePoints();

      var input = Core.GetGlobalManager<Raven.Input.InputManager>();

      // The distance between the movement of the mouse
      var _bounds = Bounds;
      var mouse = Nez.Input.RawMousePosition.ToVector2();
      var delta = mouse - input.MouseDragStart;
      delta /= Scene.Camera.RawZoom;

      // start whatever selection
      if (input.IsDragFirst)
      {
        _selectionInitial = Bounds;

        // mouse is also inside the selection area; can insead be moved
        if (_bounds.Width != 0 && _bounds.Contains(Scene.Camera.MouseToWorldPoint()))
        {
          _isDragInsideArea = true;
        }
      } 
      else if (input.IsDragLast) _hasInteraction = false;
      // released in point
      else if (IsEditingPoint && !input.IsDrag)
      {
        SelAxis = SelectionAxis.None;
      }
      else if (input.IsDragLast)
      {
        _isDragInsideArea = false;
      }
      // draggin point
      if (IsEditingPoint)
      {
        _hasInteraction = true;
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
        Bounds = _bounds;
      }
      // move selection
      else if (input.IsDrag && _isDragInsideArea)
      {
        _bounds.Location = _selectionInitial.Location + delta;
        Bounds = _bounds;
        _hasInteraction = true;
      }
      

    }
  }

}
