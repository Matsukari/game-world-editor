
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  public class Level : RenderableComponent
  {
    public string Name = "default";
    public Point ContentSize = new Point(96, 96);
    public World World { get; private set; }
    public List<Layer> Layers = new List<Layer>();
    public Layer CurrentLayer = null;
    public override RectangleF Bounds 
    { 
      get 
      {
        if (_areBoundsDirty)
        {
          _bounds.CalculateBounds(Transform.Position, _localOffset, ContentSize.ToVector2()/2f, Transform.Scale, Transform.Rotation, ContentSize.X, ContentSize.Y);
          _areBoundsDirty = true;
        }
        return _bounds;
      }
    }
    public Level(World world)
    {
      World = world;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      foreach (var layer in Layers)
      {
        layer.Draw(batcher, camera);
      }
    }
  }
}
