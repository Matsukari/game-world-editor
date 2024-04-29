using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public static class ColorExt 
  {
    public static uint ToImColor(this Color color) => ImGui.ColorConvertFloat4ToU32(color.ToVector4().ToNumerics());
    public static uint ToImColor(this Vector4 color) => ImGui.ColorConvertFloat4ToU32(color.ToNumerics());
    public static Color ToColor(this Vector4 vec) => new Color(vec);
 
  }
}
