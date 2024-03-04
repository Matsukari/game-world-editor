using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

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
  internal class SelectionRectangle : RenderableComponent
  {
    GuiData _gui;
    public int Radius = 4;
    public int SafeBuffer = 3;
    public int MouseButton = 0;
    public bool IsEditingPoint = false;
    public RectangleF Content;
    public List<RectangleF> Points = new List<RectangleF>();
    public SelectionAxis SelectionPoint = SelectionAxis.None;
    public SelectionRectangle(RectangleF rect, GuiData gui) 
    { 
      Content = rect; 
      Update();
    }
    
    public SelectionAxis Draw(Color color)
    {
      int i = 0, p = -1;
      // float delta = 0;
      foreach (var point in Points)
      {
        if (ImUtils.IsMouseAt(point.GetCenterToStart(), offset, zoom)) 
        {
          p = i;
          DrawHighlight(p, outline, offset, zoom, drawList);
        }
        drawList.AddCircleFilled(new Num.Vector2(windowMin.X + point.X * zoom, windowMin.Y + point.Y * zoom) + offset, Radius, outline.ToImColor());
        i++;
      }
      DrawHighlight((int)SelectionPoint, outline, offset, zoom, drawList);
      ImUtils.DrawRect(drawList, Content, outline, offset, zoom);
      return p != -1 ? (SelectionAxis)p : SelectionAxis.None;
    }
    void DrawHighlight(int point, Color color)
    {
      if (point >= 0 && point < (int)SelectionAxis.None) 
      {  

        drawList.AddCircleFilled(new Num.Vector2(
              windowMin.X + Points[point].X * zoom, 
              windowMin.Y + Points[point].Y * zoom) + offset, 
            Radius*2, outline.ToImColor());
      }
    }
    public void Update()
    {
      Points.Clear();
      Points.Add(new RectangleF(Content.Left , Content.Top         , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Left , Content.Bottom      , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Left , Content.Center.Y    , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Right , Content.Top        , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Right , Content.Bottom     , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Right , Content.Center.Y   , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Center.X , Content.Top     , Radius+SafeBuffer, Radius+SafeBuffer));
      Points.Add(new RectangleF(Content.Center.X , Content.Bottom  , Radius+SafeBuffer, Radius+SafeBuffer)); 
    }
    public void Snap(int w, int h)
    {
      Content.Width = MathF.Floor(Content.Width / w) * w;
      Content.Height = MathF.Floor(Content.Height / w) * w;
    }
    RectangleF _selectionInitial = new RectangleF();
    public void DrawWithInput(SpriteSheetEditor.GuiData Gui, SpriteSheetEditor Editor)
    {
      if (Gui.IsDragFirst)
      {
        _selectionInitial = Content;
      }
      SelectionAxis selectionPoint = Gui.SelectionRect.Draw(ImGui.GetForegroundDrawList(), 
          Editor.ColorSet.SelectionOutline, Gui.SheetPosition, Gui.ContentZoom);

      if (selectionPoint != SelectionAxis.None && SelectionPoint == SelectionAxis.None && ImGui.GetIO().MouseDown[0])
      {
        SelectionPoint = selectionPoint;
        IsEditingPoint = true;
      }
      else if (IsEditingPoint && !Gui.IsDrag)
      {
        IsEditingPoint = false;
        SelectionPoint = SelectionAxis.None;
      }
      // if (!IsEditingPoint) Console.WriteLine("Not editing");
      if (IsEditingPoint)
      {
        var mouse = ImGui.GetIO().MousePos;
        var delta = mouse - Gui.MouseDragStart;
        // This makes edges giigle a bit
        delta /= Gui.ContentZoom;
        // Gui.SelectionRect.Content = RectangleExt.MinMax(_selectionInitial, Gui.MouseDragArea);
        switch (SelectionPoint)
        {
          case SelectionAxis.TopLeft:
            Gui.SelectionRect.Content.X = _selectionInitial.X + delta.X;
            Gui.SelectionRect.Content.Y = _selectionInitial.Y + delta.Y;
            Gui.SelectionRect.Content.Size = _selectionInitial.Size - delta;
            break;

          case SelectionAxis.TopRight:
            Gui.SelectionRect.Content.Y = _selectionInitial.Y + delta.Y;
            Gui.SelectionRect.Content.Height = _selectionInitial.Height - delta.Y;
            Gui.SelectionRect.Content.Width = _selectionInitial.Width + delta.X;
            break;

          case SelectionAxis.BottomLeft:
            Gui.SelectionRect.Content.X = _selectionInitial.X + delta.X;
            Gui.SelectionRect.Content.Width = _selectionInitial.Width - delta.X;
            Gui.SelectionRect.Content.Height = _selectionInitial.Height + delta.Y; 
            break;

          case SelectionAxis.BottomRight: 
            Gui.SelectionRect.Content.Size = _selectionInitial.Size + delta; 
            break;

          case SelectionAxis.Right: 
            Gui.SelectionRect.Content.Width = _selectionInitial.Width + delta.X; 
            break;

          case SelectionAxis.Left: 
            Gui.SelectionRect.Content.X = _selectionInitial.X + delta.X; 
            Gui.SelectionRect.Content.Width = _selectionInitial.Width - delta.X; 
            break;

          case SelectionAxis.Top: 
            Gui.SelectionRect.Content.Y = _selectionInitial.Y + delta.Y; 
            Gui.SelectionRect.Content.Height = _selectionInitial.Height - delta.Y; 
            break;

          case SelectionAxis.Bottom: 
            Gui.SelectionRect.Content.Height = _selectionInitial.Height + delta.Y; 
            break;
        }
        // Console.WriteLine($"{selectionPoint}");
        // Console.WriteLine($"Start: {Gui.MouseDragStart}, End {mouse}, Delta {delta}");
        // Console.WriteLine($"area{Gui.MouseDragArea}");
      }
    }
  }

}
