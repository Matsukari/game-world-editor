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
    public static Color Average(this Color a, Color b) => new Color((a.R+b.R)/2, (a.G+b.G)/2, (a.B+b.B)/2, (a.A+b.A)/2);
  }
}
