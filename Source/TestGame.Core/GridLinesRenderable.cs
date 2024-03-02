
using Microsoft.Xna.Framework;

namespace Nez
{
  public class OriginLinesRenderable : RenderableComponent
  {
    public override float Width => Screen.Width;
    public override float Height => Screen.Height;
        
    public override void Render(Batcher batcher, Camera camera)
    {  
      // Horizontal line 
      batcher.DrawLine(
          new Vector2(camera.Position.X-Screen.Width, Screen.Center.Y), 
          new Vector2(camera.Position.X+Screen.Width, Screen.Center.Y), 
          Color);
      // Vertival line
      batcher.DrawLine(
          new Vector2(Screen.Center.X, camera.Position.Y-Screen.Height), 
          new Vector2(Screen.Center.X, camera.Position.Y+Screen.Height) ,
          Color);
    }

  }
  public class GridLinesRenderable : RenderableComponent
  {
    public override float Width => Screen.Width;
    public override float Height => Screen.Height;

    int _size = 0;
    public int Size { set => _size = value; }
    public GridLinesRenderable(int size)
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
