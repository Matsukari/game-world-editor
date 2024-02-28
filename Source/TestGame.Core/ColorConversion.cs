
using Microsoft.Xna.Framework;
using Num = System.Numerics;
using Nez;

namespace ImGuiNET 
{
  public class SelectionRect 
  {
    enum SelectionAreaPoint
    {
      TopLeft,
      BottomLeft,
      Left,
      TopRight,
      BottomRight,
      Right,
      Top,
      Bottom,
    }
    public SelectionRect(Rectangle rect) 
    { 
      int radius = 5;
      
      Content = rect; 
        Points.Add(new Rectangle(Content.Left , Content.Top         , radius, radius));
        Points.Add(new Rectangle(Content.Left , Content.Bottom      , radius, radius));
        Points.Add(new Rectangle(Content.Left , Content.Center.Y    , radius, radius));
        Points.Add(new Rectangle(Content.Right , Content.Top        , radius, radius));
        Points.Add(new Rectangle(Content.Right , Content.Bottom     , radius, radius));
        Points.Add(new Rectangle(Content.Right , Content.Center.Y   , radius, radius));
        Points.Add(new Rectangle(Content.Center.X , Content.Top     , radius, radius));
        Points.Add(new Rectangle(Content.Center.X , Content.Bottom  , radius, radius)); 

    }
    public Rectangle Content;
    public List<Rectangle> Points = new List<Rectangle>();
    public List<Action> PointDragAction = new List<Action>();
    public int Draw(ImDrawListPtr drawList, float radius, Color outline, Num.Vector2 offset, float zoom)
    {
      var (windowMin, windowMax) = ImUtils.GetWindowArea();
      int i = 0, p = -1;
      foreach (var point in Points)
      {
        if (ImUtils.IsMouseAt(point)) p = i;
        drawList.AddCircleFilled(new Num.Vector2(windowMin.X + point.X * zoom, windowMin.Y + point.Y * zoom), radius, ImUtils.GetColor(outline));
        i++;
      }
      ImUtils.DrawRect(drawList, Content, outline, offset, zoom);
      return p;
    }
  }
  public static partial class ImUtils 
  {
    public static uint GetColor(Color color) 
    {
      Vector4 vector = color.ToVector4();
      return ImGui.ColorConvertFloat4ToU32(new Num.Vector4(vector.X, vector.Y, vector.Z, vector.W));
    }
    public static RectangleF FloatedRect(Rectangle rectangle) 
    {
      RectangleF result = new RectangleF(
          (float)rectangle.X, (float)rectangle.Y, 
          (float)rectangle.Width, (float)rectangle.Height);
      return result;
    }
    public static bool HasMouseClickAt(Rectangle rectangle, float zoom, Num.Vector2 offset)
    {
      var (windowMin, windowMax) = GetWindowArea();
      var x = ImGui.GetIO().MouseClickedPos[0].X - windowMin.X;
      var y = ImGui.GetIO().MouseClickedPos[0].Y - windowMin.Y;

      RectangleF worldRectangle = FloatedRect(rectangle);
      worldRectangle.Size *= zoom;
      worldRectangle.X = worldRectangle.X * zoom + offset.X;
      worldRectangle.Y = worldRectangle.Y * zoom + offset.Y;

      return (x >= worldRectangle.Left && x <= worldRectangle.Right && 
              y >= worldRectangle.Top && y <= worldRectangle.Bottom);
    }
    public static bool IsMouseAt(Rectangle rectangle)
    {
      var (windowMin, windowMax) = GetWindowArea();
      var x = ImGui.GetIO().MousePos.X - windowMin.X;
      var y = ImGui.GetIO().MousePos.Y - windowMin.Y;
      return (x >= rectangle.Left && x <= rectangle.Right && 
              y >= rectangle.Top && y <= rectangle.Bottom);
    }
    public static void DrawRect(ImDrawListPtr drawList, Rectangle rect, Color outline) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRect(
          new Num.Vector2(windowMin.X + rect.X, windowMin.Y + rect.Y), 
          new Num.Vector2(windowMin.X + rect.Right, windowMin.Y + rect.Bottom), 
          GetColor(outline));
    }
    public static void DrawRect(ImDrawListPtr drawList, Rectangle rect, Color outline, Num.Vector2 offset, float zoom) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRect(
          new Num.Vector2(windowMin.X + rect.X * zoom, windowMin.Y + rect.Y * zoom) + offset, 
          new Num.Vector2(windowMin.X + rect.Right * zoom, windowMin.Y + rect.Bottom * zoom) + offset, 
          GetColor(outline));
    }
    public static (Num.Vector2, Num.Vector2) GetWindowArea() 
    {
      Num.Vector2 vMin = ImGui.GetWindowContentRegionMin();
      Num.Vector2 vMax = ImGui.GetWindowContentRegionMax();
      vMin += ImGui.GetWindowPos();
      vMax += ImGui.GetWindowPos();
      return (vMin, vMax);
    }
    public static Rectangle GetWindowRect() 
    {
      Num.Vector2 vMin = ImGui.GetWindowContentRegionMin();
      Num.Vector2 vMax = ImGui.GetWindowContentRegionMax();
      vMin += ImGui.GetWindowPos();
      vMax += ImGui.GetWindowPos();
      return new Rectangle((int)vMin.X, (int)vMin.Y, (int)(vMax.X-vMin.X), (int)(vMax.Y-vMin.Y));
    }
     
  }
}
