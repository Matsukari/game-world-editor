using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  public class Layer
  {
    public Level Level;
    public string Name = "Layer 1";
    public float Opacity = 1f;
    public bool IsVisible = true;
    public bool IsLocked = false;
    public Vector2 Offset = new Vector2();
    public Layer(Level level)
    {
      Level = level;
    }
    public virtual void Draw(Batcher batcher, Camera camera)
    {
    }
  }

}
