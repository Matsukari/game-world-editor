using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;

namespace Raven.Utils
{
  public static class Input
  {
    public static bool IsMouseAt(RectangleF rectangle)
    {
      var x = ImGui.GetIO().MousePos.X;
      var y = ImGui.GetIO().MousePos.Y;

      return (x >= rectangle.Left && x <= rectangle.Right && 
          y >= rectangle.Top && y <= rectangle.Bottom);
    }
  }

}
