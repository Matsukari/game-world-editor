
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Guidelines
{
  public class OriginLinesRenderable : RenderableComponent
  {
    public override float Width => Screen.Width;
    public override float Height => Screen.Height;
        
    public override void Render(Batcher batcher, Camera camera)
    {  
      // Horizontal line 
      batcher.DrawLine(
          new Vector2(camera.Bounds.Left, 0), 
          new Vector2(camera.Bounds.Right, 0), 
          color: Color,
          thickness: 1/camera.RawZoom);
      // Vertival line
      batcher.DrawLine(
          new Vector2(0, camera.Bounds.Top), 
          new Vector2(0, camera.Bounds.Bottom), 
          color: Color,
          thickness: 1/camera.RawZoom);
    }
    public static void Render(Batcher batcher, Camera camera, Color xColor, Color yColor)
    {
      // Horizontal line 
      batcher.DrawLine(
          new Vector2(camera.Bounds.Left, 0), 
          new Vector2(camera.Bounds.Right, 0), 
          color: xColor,
          thickness: 1/camera.RawZoom);
      // Vertival line
      batcher.DrawLine(
          new Vector2(0, camera.Bounds.Top), 
          new Vector2(0, camera.Bounds.Bottom), 
          color: yColor,
          thickness: 1/camera.RawZoom);
    }

  }
}
