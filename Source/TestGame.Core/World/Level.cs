
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  public class Level : RenderableComponent, IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "default";
    public Point ContentSize = new Point(Screen.Width, Screen.Height);
    public World World { get; private set; }
    public List<Layer> Layers = new List<Layer>();
    public Layer CurrentLayer = null;
    public bool IsLocked { get; set; }
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
