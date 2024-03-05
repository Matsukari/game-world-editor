using Nez;
using Microsoft.Xna.Framework;

namespace Raven 
{
  public static class BatcherDrawing
  {
    public static void DrawRectOutline(this Batcher batcher, RectangleF rect, Color color)
    {
      batcher.DrawPoints(new Vector2(), new []{rect.Location, rect.TopRight(), rect.BottomRight(), rect.BottomLeft()}, color, true);
    }
  }
}
