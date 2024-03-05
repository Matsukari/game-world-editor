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
    GuiData _gui;
    Editor _editor;
    public int MouseButton = 0;
    public bool IsEditingPoint = false;
    public SelectionAxis SelAxis = SelectionAxis.None;
    public Renderable Ren { get; private set; }
    public Selection(GuiData gui, Editor editor=null) 
    { 
      _gui = gui;
      _editor = editor;
    }
    public class Renderable : Editor.SubEntity.RenderableComponent<Selection>
    {
      public int Radius = 4;
      public int SafeBuffer = 3;
      public List<RectangleF> Points = new List<RectangleF>();
      public override void Render(Batcher batcher, Camera camera)
      {
        if (!IsVisible) return;
        int axis = 0, selAxis = -1;
        // float delta = 0;
        foreach (var point in Points)
        {
          if (Utils.Input.IsMouseAt(point.GetCenterToStart())) 
          {
            selAxis = axis;
          }
          batcher.DrawCircle(point.Location, Radius, Color);
          axis++;
        }
        LargenSelectionAxis((int)Parent.SelAxis, Color, batcher);
        batcher.DrawRect(Bounds, Color);
        var selectionPoint = axis != -1 ? (SelectionAxis)axis : SelectionAxis.None;
        if (selectionPoint != SelectionAxis.None && Parent.SelAxis == SelectionAxis.None && ImGui.GetIO().MouseDown[0])
        {
          Parent.SelAxis = selectionPoint;
          Parent.IsEditingPoint = true;
        }
      }
      public void UpdatePoints()
      {
        Points.Clear();
        Points.Add(new RectangleF(Bounds.Left , Bounds.Top         , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Left , Bounds.Bottom      , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Left , Bounds.Center.Y    , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Right , Bounds.Top        , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Right , Bounds.Bottom     , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Right , Bounds.Center.Y   , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Center.X , Bounds.Top     , Radius+SafeBuffer, Radius+SafeBuffer));
        Points.Add(new RectangleF(Bounds.Center.X , Bounds.Bottom  , Radius+SafeBuffer, Radius+SafeBuffer)); 
      }
      void LargenSelectionAxis(int point, Color color, Batcher batcher)
      {
        if (point >= 0 && point < (int)SelectionAxis.None) 
        {  
          batcher.DrawCircle(Points[point].Location, Radius*1.5f, color);  
        }
      }
      public void Snap(int w, int h)
      {
        _bounds.Width = MathF.Floor(Bounds.Width / w) * w;
        _bounds.Height = MathF.Floor(Bounds.Height / w) * w;
      }
    }
    RectangleF _selectionInitial = new RectangleF();
    public override void Update()
    {
      base.Update();
      var input = Core.GetGlobalManager<Raven.Input.InputManager>();
      if (input.IsDragFirst)
      {
        _selectionInitial = Ren.Bounds;
      }
      else if (IsEditingPoint && !input.IsDrag)
      {
        IsEditingPoint = false;
        SelAxis = SelectionAxis.None;
      }
      if (IsEditingPoint)
      {
        var mouse = ImGui.GetIO().MousePos;
        var delta = mouse - input.MouseDragStart;
        delta /= _gui.Zoom;
        var _bounds = Ren.Bounds;
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
        Ren.SetBounds(_bounds);
        // Console.WriteLine($"{selectionPoint}");
        // Console.WriteLine($"Start: {Gui.MouseDragStart}, End {mouse}, Delta {delta}");
        // Console.WriteLine($"area{Gui.MouseDragArea}");
        Update();
      }
    }
  }

}
