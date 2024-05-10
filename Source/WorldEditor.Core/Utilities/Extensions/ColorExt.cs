using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;

namespace Raven
{
  public static class ColorExt 
  {
    public static uint ToImColor(this Color color) => ImGui.ColorConvertFloat4ToU32(color.ToVector4().ToNumerics());
    public static uint ToImColor(this Vector4 color) => ImGui.ColorConvertFloat4ToU32(color.ToNumerics());
    public static Color ToColor(this Vector4 vec) => new Color(vec);
    public static Color Average(this Color a, params Color[] b) 
    {
      var combined = a;
      foreach (var item in b)
      {
        combined.Add(new Color((combined.R+item.R), (combined.G+item.G), (combined.B+item.B), (combined.A+item.A)));
      }
      var count = b.Count() + 1;
      return new Color((int)combined.R/count, (int)combined.G/count, (int)combined.B/count, (int)combined.A/count);
    }
  }
}
