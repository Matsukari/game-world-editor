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
    public static void DrawRectOutline(this Batcher batcher, Camera camera, RectangleF rect, Color color, float thickness=1f)
    {
      batcher.DrawPoints(new Vector2(), new []{rect.Location, rect.TopRight(), rect.BottomRight(), rect.BottomLeft()}, color, true, thickness/camera.RawZoom);
    }
    public static void DrawStringCentered(this Batcher batcher, Camera camera, string input, Vector2 position, Color color, Vector2 zoomRange, 
        bool centerX, bool centerY)
    {
      if (centerX) position.X -= Graphics.Instance.BitmapFont.MeasureString(input).X;
      if (centerY) position.Y -= Graphics.Instance.BitmapFont.MeasureString(input).Y;
      batcher.DrawString(camera, input, position, color, zoomRange);
    }

    public static void DrawString(this Batcher batcher, Camera camera, string input, Vector2 position, Color color, Vector2 zoomRange)
    {
      batcher.DrawString(
          Graphics.Instance.BitmapFont, 
          input,
          position,
          color: color, 
          rotation: 0f, 
          origin: Vector2.Zero, 
          scale: Math.Clamp(zoomRange.X/camera.RawZoom, zoomRange.X, zoomRange.Y), 
          effects: Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 
          layerDepth: 0f);
    }
  }
}
