using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;
using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Raven
{
  public static partial class ImUtils 
  {
    private static IntPtr _lastTexture = IntPtr.Zero;
    public static IntPtr BindImGui(this Texture2D texture) 
    {        
        if (_lastTexture != IntPtr.Zero) Core.GetGlobalManager<ImGuiManager>().UnbindTexture(_lastTexture);
        _lastTexture = Core.GetGlobalManager<ImGuiManager>().BindTexture(texture);
        return _lastTexture;
    }
    public static void UnbindLastTexture() 
    {        
        if (_lastTexture != IntPtr.Zero) Core.GetGlobalManager<ImGuiManager>().UnbindTexture(_lastTexture);
    }
    public static uint ToImColor(this Color color) 
    {
      return ImGui.ColorConvertFloat4ToU32(color.ToVector4().ToNumerics());
    }
    public static bool HasMouseClickAt(Rectangle rectangle, float zoom, Num.Vector2 offset)
    {
      var (windowMin, windowMax) = GetWindowArea();
      var x = ImGui.GetIO().MouseClickedPos[0].X - windowMin.X;
      var y = ImGui.GetIO().MouseClickedPos[0].Y - windowMin.Y;
      RectangleF worldRectangle = rectangle.ToRectangleF();
      worldRectangle.Size *= zoom;
      worldRectangle.X = worldRectangle.X * zoom + offset.X;
      worldRectangle.Y = worldRectangle.Y * zoom + offset.Y;

      return (x >= worldRectangle.Left && x <= worldRectangle.Right && 
              y >= worldRectangle.Top && y <= worldRectangle.Bottom);
    }
    public static bool HasMouseRealClickAt(Rectangle rectangle)
    {
      var x = ImGui.GetIO().MouseClickedPos[0].X;
      var y = ImGui.GetIO().MouseClickedPos[0].Y;
      RectangleF worldRectangle = rectangle.ToRectangleF();

      return (x >= worldRectangle.Left && x <= worldRectangle.Right && 
              y >= worldRectangle.Top && y <= worldRectangle.Bottom);
    }
    public static bool HasMouseClickAt(Rectangle rectangle) => HasMouseClickAt(rectangle, 1, new Num.Vector2());
    public static bool IsMouseAt(Rectangle rectangle, Num.Vector2 offset=new Num.Vector2(), float zoom=1f)
    {
       var (windowMin, windowMax) = GetWindowArea();
      var x = ImGui.GetIO().MousePos.X - windowMin.X;
      var y = ImGui.GetIO().MousePos.Y - windowMin.Y;

      RectangleF worldRectangle = rectangle.ToRectangleF();
      worldRectangle.Size *= zoom;
      worldRectangle.X = worldRectangle.X * zoom + offset.X;
      worldRectangle.Y = worldRectangle.Y * zoom + offset.Y;

      return (x >= worldRectangle.Left && x <= worldRectangle.Right && 
              y >= worldRectangle.Top && y <= worldRectangle.Bottom);
    }
    public static Num.Vector2 Translate(Num.Vector2 point, float zoom, Num.Vector2 offset)
    {
       var (windowMin, windowMax) = GetWindowArea();
       return new Num.Vector2(windowMin.X + point.X * zoom, windowMin.Y + point.Y * zoom) + offset;
    }
    public static void DrawRealRect(ImDrawListPtr drawList, Rectangle rect, Color outline) 
    {
      drawList.AddRect(
          new Num.Vector2(rect.X, rect.Y), 
          new Num.Vector2(rect.Right, rect.Bottom), 
          outline.ToImColor());
    }
    public static void DrawRect(ImDrawListPtr drawList, Rectangle rect, Color outline) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRect(
          new Num.Vector2(windowMin.X + rect.X, windowMin.Y + rect.Y), 
          new Num.Vector2(windowMin.X + rect.Right, windowMin.Y + rect.Bottom), 
          outline.ToImColor());
    }
    public static void DrawRect(ImDrawListPtr drawList, RectangleF rect, Color outline, Num.Vector2 offset, float zoom) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRect(
          new Num.Vector2(windowMin.X + rect.X * zoom, windowMin.Y + rect.Y * zoom) + offset, 
          new Num.Vector2(windowMin.X + rect.Right * zoom, windowMin.Y + rect.Bottom * zoom) + offset, 
          outline.ToImColor());
    }
    public static void DrawRectFilled(ImDrawListPtr drawList, Rectangle rect, Color fill, Num.Vector2 offset, float zoom) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRectFilled(
          new Num.Vector2(windowMin.X + rect.X * zoom, windowMin.Y + rect.Y * zoom) + offset, 
          new Num.Vector2(windowMin.X + rect.Right * zoom, windowMin.Y + rect.Bottom * zoom) + offset, 
          fill.ToImColor());
    }
    public static void DrawArrowDownFilled(ImDrawListPtr drawList, Num.Vector2 point, float size, Color fill, Num.Vector2 offset, float zoom) 
    {
      var circle = point;
      circle.Y -= size + size*0.4f;
      DrawCircleFilled(drawList, circle, size/2, fill, offset, zoom);
      

      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddTriangleFilled(
          new Num.Vector2(windowMin.X + (circle.X - size/2) * zoom, windowMin.Y + (circle.Y) * zoom) + offset, //left 
          new Num.Vector2(windowMin.X + (circle.X + size/2) * zoom, windowMin.Y + (circle.Y) * zoom) + offset, // right 
          new Num.Vector2(windowMin.X + (circle.X) * zoom, windowMin.Y + (circle.Y + size + size*0.4f) * zoom) + offset, // bottom
          fill.ToImColor());
    }

    public static void DrawCircleFilled(ImDrawListPtr drawList, Num.Vector2 center, float radius, Color fill, Num.Vector2 offset, float zoom) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddCircleFilled(
          new Num.Vector2(windowMin.X + center.X * zoom, windowMin.Y + center.Y * zoom) + offset,
          radius * zoom, fill.ToImColor());
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
    public static void LabelText(string label, string value) => ImGui.LabelText(value, label);
  } 
}
