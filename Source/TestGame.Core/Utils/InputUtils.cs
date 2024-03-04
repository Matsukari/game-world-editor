using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;

namespace Raven.Utils
{
  public static class Input
  {
    public static bool IsMouseAt(Rectangle rectangle)
    {
      var x = ImGui.GetIO().MousePos.X;
      var y = ImGui.GetIO().MousePos.Y;

      RectangleF worldRectangle = rectangle.ToRectangleF();
      worldRectangle.Size *= zoom;
      worldRectangle.X = worldRectangle.X * zoom + offset.X;
      worldRectangle.Y = worldRectangle.Y * zoom + offset.Y;

      return (x >= worldRectangle.Left && x <= worldRectangle.Right && 
          y >= worldRectangle.Top && y <= worldRectangle.Bottom);
    }
  }

}
