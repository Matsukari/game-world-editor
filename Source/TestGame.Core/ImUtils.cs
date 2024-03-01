using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;
using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Tools 
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
    public static void DrawRect(ImDrawListPtr drawList, Rectangle rect, Color outline) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRect(
          new Num.Vector2(windowMin.X + rect.X, windowMin.Y + rect.Y), 
          new Num.Vector2(windowMin.X + rect.Right, windowMin.Y + rect.Bottom), 
          outline.ToImColor());
    }
    public static void DrawRect(ImDrawListPtr drawList, Rectangle rect, Color outline, Num.Vector2 offset, float zoom) 
    {
      var (windowMin, windowMax) = GetWindowArea();
      drawList.AddRect(
          new Num.Vector2(windowMin.X + rect.X * zoom, windowMin.Y + rect.Y * zoom) + offset, 
          new Num.Vector2(windowMin.X + rect.Right * zoom, windowMin.Y + rect.Bottom * zoom) + offset, 
          outline.ToImColor());
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
