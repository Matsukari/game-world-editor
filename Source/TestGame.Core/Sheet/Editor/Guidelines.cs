
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
          new Vector2(-10000, 0), 
          new Vector2(10000 + Screen.Width, 0), 
          color: Color,
          thickness: 1/camera.RawZoom);
      // Vertival line
      batcher.DrawLine(
          new Vector2(0, -10000), 
          new Vector2(0, 10000), 
          color: Color,
          thickness: 1/camera.RawZoom);
    }

  }
  public class GridLines : RenderableComponent
  {
    public override float Width => Screen.Width;
    public override float Height => Screen.Height;

    int _size = 0;
    public int Size { set => _size = value; }
    public GridLines(int size)
    {
      _size = size;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      int cols = (int)(Screen.Width / _size);
      int rows = (int)(Screen.Height / _size);
       
      for (int x = 0; x <= cols; x++) 
      {
        // Vertival lines
        batcher.DrawLine(new Vector2(x * _size, 0f) + camera.Position, new Vector2(x * _size, rows * _size) + camera.Position, Color);
        for (int y = 0; y <= rows; y++) 
        {
          // Horizontal lines
          batcher.DrawLine(new Vector2(0f, y * _size) + camera.Position, new Vector2(cols * _size, y * _size) + camera.Position, Color);
        }
      }
    }
  }
}
