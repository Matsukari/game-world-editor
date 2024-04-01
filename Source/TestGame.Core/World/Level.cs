
using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven.Sheet
{
  /// <summary>
  /// Composed of Layers of the same size as this.
  /// </summary>
  public class Level : Entity, IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    public Point ContentSize = new Point(Screen.Width, Screen.Height);
    public World World { get; private set; }
    public List<Layer> Layers = new List<Layer>();
    public Layer CurrentLayer = null;
    public bool IsLocked { get; set; }

    public RectangleF Bounds 
    { 
      get 
      {
        _bounds.CalculateBounds(Transform.Position, Vector2.Zero, ContentSize.ToVector2()/2f, Transform.Scale, Transform.Rotation, ContentSize.X, ContentSize.Y);
        return _bounds;
      }
    }

    RectangleF _bounds = new RectangleF();

    public Level(World world)
    {
      World = world;
    }
    public void Render(Batcher batcher, Camera camera)
    {
      foreach (var layer in Layers)
      {
        layer.Draw(batcher, camera);
      }
    }
  }
}
