using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Raven
{
  public static class Vector2Ext 
  {
    public static bool IsDoubleDirection(this SelectionAxis axis)
      => axis == SelectionAxis.TopRight || axis == SelectionAxis.TopLeft || axis == SelectionAxis.BottomLeft || axis == SelectionAxis.BottomRight;

    public static bool HasTopOrLeft(this SelectionAxis axis) 
      => axis == SelectionAxis.Left 
      || axis == SelectionAxis.Top 
      || axis == SelectionAxis.TopLeft 
      || axis == SelectionAxis.TopRight 
      || axis == SelectionAxis.BottomLeft;

    public static (SelectionAxis, SelectionAxis) SeparateDouble(this SelectionAxis axis)
    {
      if (axis == SelectionAxis.TopRight) return (SelectionAxis.Top, SelectionAxis.Right);
      else if (axis == SelectionAxis.TopLeft) return (SelectionAxis.Top, SelectionAxis.Left);
      else if (axis == SelectionAxis.BottomLeft) return (SelectionAxis.Bottom, SelectionAxis.Left);
      else if (axis == SelectionAxis.BottomRight) return (SelectionAxis.Bottom, SelectionAxis.Right);

      return (SelectionAxis.None, SelectionAxis.None);
    }

    public static Point DirectionFactor(this SelectionAxis axis)
    {
      if (axis.IsDoubleDirection() || axis == SelectionAxis.None) throw new Exception();
      if (axis == SelectionAxis.Left ||axis == SelectionAxis.Right) return new Point(1, 0);
      return new Point(0, 1);
    }

    public static bool IsXAxis(this SelectionAxis axis)
    {
      if (axis.IsDoubleDirection()) throw new Exception();
      if (axis == SelectionAxis.Left || axis == SelectionAxis.Right) return true;
      return false;
    }

    public static string SimpleStringFormat(this Vector2 vec)
    {
      return $"{vec.X}, {vec.Y}";
    }
    public static string SimpleStringFormat(this Point vec)
    {
      return $"{vec.X}, {vec.Y}";
    }
    public static int CompareInGridSpace(this Point p1, Point p2)
    {
      if (p1.Y != p2.Y)
        return p1.Y.CompareTo(p2.Y);
      return p1.X.CompareTo(p2.X);
    }
    public static Vector2 EaseTo(Vector2 from, Vector2 to, float ease) => 
      new Vector2(
          from.X + ease * (to.X - from.X),
          from.Y + ease * (to.Y - from.Y)
          );
    public static bool EitherIsNegative(this Vector2 vector) => vector.X < 0 || vector.Y < 0;
    public static bool EitherIsNegative(this Point point) => point.X < 0 || point.Y < 0;

    public static Vector2 Negate(this Vector2 vector) => new Vector2(-vector.X, -vector.Y);

    public static Vector2 ToVector2(this Num.Vector2 numeric)
    {
      return new Vector2(numeric.X, numeric.Y);
    }
    public static Vector4 ToVector4(this Num.Vector4 numeric)
    {
      return new Vector4(numeric.X, numeric.Y, numeric.Z, numeric.W);
    }
    public static Vector2 Divide(this Vector2 vec, int x, int y)
    {
      return new Vector2(vec.X/x, vec.Y/y);
    }
    public static Point Divide(this Point vec, int x, int y)
    {
      return new Point(vec.X/x, vec.Y/y);
    }
    public static Vector2 MathMax(this Vector2 a, Vector2 b)
    {
      var result = new Vector2();
      result.X = Math.Max(a.X, b.X);
      result.Y = Math.Max(a.Y, b.Y);
      return result;
    }
    public static Vector2 MathMin(this Vector2 a, Vector2 b)
    {
      var result = new Vector2();
      result.X = Math.Min(a.X, b.X);
      result.Y = Math.Min(a.Y, b.Y);
      return result;
    }
    public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max)
    {
      return new Vector2(Math.Clamp(vec.X, min.X, max.X), Math.Clamp(vec.Y, min.Y, max.Y));
    }
    public static Point Max(this Point min, Point max)
    {
      if (min.X > max.X || min.Y > max.Y) return min;
      return max;
    }

    public static Vector2 RoundFloor(this Vector2 vec, Point point)
    {
      vec.X = (int)(vec.X / point.X) * point.X;
      vec.Y = (int)(vec.Y / point.Y) * point.Y;
      return vec;
    }
    public static Vector2[] AddAsPositionToVertices(this Vector2 offset, List<Vector2> vertices)
    {
      var s = new List<Vector2>();
      foreach (var vertex in vertices) s.Add(offset + vertex);
      return s.ToArray();
    }
    public static Vector2[] AddAsPositionToVertices(this Vector2 offset, Vector2[] vertices)
    {
      var s = new List<Vector2>();
      foreach (var vertex in vertices) s.Add(offset + vertex);
      return s.ToArray();
    }
  }
}
