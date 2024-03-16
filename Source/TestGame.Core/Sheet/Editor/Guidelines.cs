
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Guidelines
{
  public class OriginLines : RenderableComponent
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

  }
  public class GridLines : RenderableComponent
  {
    public override RectangleF Bounds 
    {
      get
      {
        if (_areBoundsDirty)
        {
          _bounds.CalculateBounds(Entity.Position, _localOffset, new Vector2(Lines.X * Margin.X, Lines.Y * Margin.Y)/2, 
              Entity.Scale, Entity.Rotation, Lines.X * Margin.X, Lines.Y * Margin.Y); 
          _areBoundsDirty = false;
        }
        return _bounds;
      }
    }
    public Point Lines = new Point();
    public Vector2 Margin = new Vector2();
    public override void Render(Batcher batcher, Camera camera)
    {
      for (int x = 0; x <= Lines.X; x++) 
      {
        // Vertival lines
        batcher.DrawLine(
            new Vector2(x * Margin.X, 0f) + Bounds.Location, 
            new Vector2(x * Margin.X, Lines.Y * Margin.Y) + Bounds.Location, 
            Color);
        for (int y = 0; y <= Lines.Y; y++) 
        {
          // Horizontal lines
          batcher.DrawLine(
              new Vector2(0f, y * Margin.Y) + Bounds.Location, 
              new Vector2(Lines.X * Margin.X, y * Margin.Y) + Bounds.Location, 
              Color);
        }
      }
    }
  }
}
