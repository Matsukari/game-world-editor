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
    public static Vector2[] AddPositionVertices(List<Vector2> vertices, Vector2 offset)
    {
      var s = new List<Vector2>();
      foreach (var vertex in vertices) s.Add(offset + vertex);
      return s.ToArray();
    }
  }
}
