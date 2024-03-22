using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Raven
{
  public static class RectangleExt 
  {
    public static Vector2 LeftCenter(this RectangleF rect)
    {
      rect.Y = rect.Center.Y;
      return rect.Location;
    }
    public static Vector2 TopRight(this RectangleF rect)
    {
      rect.X = rect.Right;
      return rect.Location;
    }
    public static Vector2 BottomRight(this RectangleF rect)
    {
      rect.X = rect.Right;
      rect.Y = rect.Bottom;
      return rect.Location;
    }
    public static Vector2 BottomLeft(this RectangleF rect)
    {
      rect.Y = rect.Bottom;
      return rect.Location;
    }
    public static Vector2 BottomCenter(this RectangleF rect)
    {
      rect.Y = rect.Bottom;
      rect.X = rect.Center.X;
      return rect.Location;
    }
    public static RectangleF ConsumePoint(this RectangleF rectangle, System.Numerics.Vector2 pos)
    {
      rectangle.X = Math.Min(rectangle.X, pos.X);
      rectangle.Y = Math.Min(rectangle.Y, pos.Y);
      rectangle.Width = Math.Abs(rectangle.Width);
      rectangle.Height = Math.Abs(rectangle.Height);
      return rectangle;
    }
    public static RectangleF AlwaysPositive(this RectangleF rectangle)
    {
      rectangle.X = Math.Min(rectangle.X, rectangle.X + rectangle.Width);
      rectangle.Y = Math.Min(rectangle.Y, rectangle.Y + rectangle.Height);
      rectangle.Width = Math.Abs(rectangle.Width);
      rectangle.Height = Math.Abs(rectangle.Height);
      return rectangle;
    }
    public static RectangleF MinMax(RectangleF a, RectangleF b)
    {
      var min = new Vector2();
      var max = new Vector2();  
      min.X = Math.Min(a.X, b.X);
      min.Y = Math.Min(a.Y, b.Y);
      max.X = Math.Max(a.Right, b.Right);
      max.Y = Math.Max(a.Bottom, b.Bottom);
      return RectangleF.FromMinMax(min, max);
    }
    public static RectangleF AddPosition(this RectangleF rect, Vector2 delta)
    {
      rect.Location += delta;
      return rect;
    }
    public static RectangleF MinMax(List<RectangleF> rects)
    {
      var min = new Vector2(10000, 10000);
      var max = new Vector2(-10000, -10000);  
      if (rects.Count == 0) return RectangleF.Empty;
      foreach (var rect in rects)
      {
        min.X = Math.Min(min.X, rect.X);
        min.Y = Math.Min(min.Y, rect.Y);
        max.X = Math.Max(max.X, rect.Right);
        max.Y = Math.Max(max.Y, rect.Bottom);
      }
      return RectangleF.FromMinMax(min, max);
    }
    public static Rectangle MinMax(List<Rectangle> rects)
    {
      var min = new Vector2(10000, 10000);
      var max = new Vector2(-10000, -10000);  
      if (rects.Count == 0) return RectangleF.Empty;
      foreach (var rect in rects)
      {
        min.X = Math.Min(min.X, rect.X);
        min.Y = Math.Min(min.Y, rect.Y);
        max.X = Math.Max(max.X, rect.Right);
        max.Y = Math.Max(max.Y, rect.Bottom);
      }
      return RectangleF.FromMinMax(min, max);
    }
    public static RectangleF ToRectangleF(this Rectangle rect) 
    {
      RectangleF result = new RectangleF(
          (float)rect.X, (float)rect.Y, 
          (float)rect.Width, (float)rect.Height);
      return result;
    }
    public static RectangleF ToRectangleF(this Num.Vector4 vec) 
    {
      RectangleF result = new RectangleF(
          vec.X, vec.Y, 
          vec.Z, vec.W);
      return result;
    }
    public static Num.Vector4 ToNumerics(this RectangleF rect)
    {
      return new Num.Vector4(rect.X, rect.Y, rect.Width, rect.Height);
    }
    public static RectangleF GetCenterToStart(this RectangleF rectf) 
    {
      RectangleF rect = rectf;
      rect.X -= rect.Width/2;
      rect.Y -= rect.Height/2;
      return rect;
    }
    public static void SetCenterToStart(this RectangleF rect) 
    {
      rect.X -= rect.Width/2;
      rect.Y -= rect.Height/2;
    }
    public static string RenderStringFormat(this Rectangle rect) => $"{rect.X}, {rect.Y}, {rect.Width}, {rect.Height}";
    public static string RenderStringFormat(this RectangleF rect) => $"{rect.X}, {rect.Y}, {rect.Width}, {rect.Height}";

  }
}
