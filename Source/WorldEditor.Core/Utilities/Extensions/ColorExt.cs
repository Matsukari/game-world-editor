using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public static class ColorExt 
  {
    public static uint ToImColor(this Color color) => ImGui.ColorConvertFloat4ToU32(color.ToVector4().ToNumerics());
    public static uint ToImColor(this Vector4 color) => ImGui.ColorConvertFloat4ToU32(color.ToNumerics());
    public static Color ToColor(this Vector4 vec) => new Color(vec);
    public static Color Average(this Color a, params Color[] b) 
    { 
      var combined = a.ToVector4() * 255f;
      foreach (var item in b)
      {
        combined += item.ToVector4() * 255f;
      }
      var count = b.Count() + 1;
      return new Color((int)(combined.X/count), (int)(combined.Y/count), (int)(combined.Z/count), (int)(combined.W/count));
    }
  }
}
