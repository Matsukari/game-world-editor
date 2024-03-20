using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;
using Nez;
using Nez.ImGuiTools;
using ImGuiNET;

namespace Raven
{
  public static class Vector2Ext 
  {
    public static string SimpleStringFormat(this Vector2 vec)
    {
      return $"{vec.X}, {vec.Y}";
    }
    public static string SimpleStringFormat(this Point vec)
    {
      return $"{vec.X}, {vec.Y}";
    }
    public static Vector2 ToVector2(this Num.Vector2 numeric)
    {
      return new Vector2(numeric.X, numeric.Y);
    }
    public static Vector2 Divide(this Vector2 vec, int x, int y)
    {
      return new Vector2(vec.X/x, vec.Y/y);
    }
    public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max)
    {
      return new Vector2(Math.Clamp(vec.X, min.X, max.X), Math.Clamp(vec.Y, min.Y, max.Y));
    }

    public static Vector2[] AddPositionVertices(List<Vector2> vertices, Vector2 offset)
    {
      var s = new List<Vector2>();
      foreach (var vertex in vertices) s.Add(offset + vertex);
      return s.ToArray();
    }
  }
}
