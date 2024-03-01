using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Tools 
{
  public enum SelectionAreaPoint
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
  public class SelectionRectangle 
  {
    SpriteSheetEditor.GuiData _gui;
    public int Radius = 4;
    public int SafeBuffer = 3;
    public int MouseButton = 0;
    public bool IsEditingPoint = false;
    public RectangleF Content;
    public List<RectangleF> Points = new List<RectangleF>();
    public SelectionAreaPoint SelectionPoint = SelectionAreaPoint.None;
    public SelectionRectangle(RectangleF rect, SpriteSheetEditor.GuiData gui) 
    { 
      Content = rect; 
      Update();
    }
    public SelectionAreaPoint Draw(ImDrawListPtr drawList, Color outline, Num.Vector2 offset, float zoom)
    {
      var (windowMin, windowMax) = ImUtils.GetWindowArea();
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
      return p != -1 ? (SelectionAreaPoint)p : SelectionAreaPoint.None;
    }
    void DrawHighlight(int point, Color outline, Num.Vector2 offset, float zoom, ImDrawListPtr drawList)
    {
      var (windowMin, windowMax) = ImUtils.GetWindowArea();
      if (point >= 0 && point < (int)SelectionAreaPoint.None) 
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
    RectangleF _selectionInitial = new RectangleF();
    public void DrawWithInput(SpriteSheetEditor.GuiData Gui, SpriteSheetEditor Editor)
    {
      if (Gui.IsDragFirst)
      {
        _selectionInitial = Content;
      }
      SelectionAreaPoint selectionPoint = Gui.SelectionRect.Draw(ImGui.GetForegroundDrawList(), 
          Editor.ColorSet.SelectionOutline, Gui.SheetPosition, Gui.ContentZoom);

      if (selectionPoint != SelectionAreaPoint.None && SelectionPoint == SelectionAreaPoint.None && ImGui.GetIO().MouseDown[0])
      {
        SelectionPoint = selectionPoint;
        IsEditingPoint = true;
      }
      else if (IsEditingPoint && !Gui.IsDrag)
      {
        IsEditingPoint = false;
        SelectionPoint = SelectionAreaPoint.None;
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
          case SelectionAreaPoint.TopLeft:
            Gui.SelectionRect.Content.X = _selectionInitial.X + delta.X;
            Gui.SelectionRect.Content.Y = _selectionInitial.Y + delta.Y;
            Gui.SelectionRect.Content.Size = _selectionInitial.Size - delta;
            break;

          case SelectionAreaPoint.TopRight:
            Gui.SelectionRect.Content.Y = _selectionInitial.Y + delta.Y;
            Gui.SelectionRect.Content.Height = _selectionInitial.Height - delta.Y;
            Gui.SelectionRect.Content.Width = _selectionInitial.Width + delta.X;
            break;

          case SelectionAreaPoint.BottomLeft:
            Gui.SelectionRect.Content.X = _selectionInitial.X + delta.X;
            Gui.SelectionRect.Content.Width = _selectionInitial.Width - delta.X;
            Gui.SelectionRect.Content.Height = _selectionInitial.Height + delta.Y; 
            break;

          case SelectionAreaPoint.BottomRight: 
            Gui.SelectionRect.Content.Size = _selectionInitial.Size + delta; 
            break;

          case SelectionAreaPoint.Right: 
            Gui.SelectionRect.Content.Width = _selectionInitial.Width + delta.X; 
            break;

          case SelectionAreaPoint.Left: 
            Gui.SelectionRect.Content.X = _selectionInitial.X + delta.X; 
            Gui.SelectionRect.Content.Width = _selectionInitial.Width - delta.X; 
            break;

          case SelectionAreaPoint.Top: 
            Gui.SelectionRect.Content.Y = _selectionInitial.Y + delta.Y; 
            Gui.SelectionRect.Content.Height = _selectionInitial.Height - delta.Y; 
            break;

          case SelectionAreaPoint.Bottom: 
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
