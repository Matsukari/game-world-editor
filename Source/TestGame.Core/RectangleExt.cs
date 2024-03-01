using Nez;
using Microsoft.Xna.Framework;

namespace Tools 
{
  public static class RectangleExt 
  {
    public static RectangleF ConsumePoint(this RectangleF rectangle, System.Numerics.Vector2 pos)
    {
      rectangle.X = Math.Min(rectangle.X, pos.X);
      rectangle.Y = Math.Min(rectangle.Y, pos.Y);
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
    public static RectangleF ToRectangleF(this Rectangle rect) 
    {
      RectangleF result = new RectangleF(
          (float)rect.X, (float)rect.Y, 
          (float)rect.Width, (float)rect.Height);
      return result;
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
  }
}
