using Nez;

namespace Raven
{
  public class LayerRenderer<T> : RenderableComponent where T: Layer
  {
    public T Layer;
    public WorldEntity World;
    public LevelEntity Level;
    public void Initialize(T layer)
    {
      Layer = layer;
    }
    public override RectangleF Bounds 
    { 
      get 
      {
        _bounds.CalculateBounds(
            Transform.Position + Layer.Level.Bounds.Location, (Layer.Size.ToVector2()/2) + Layer.Offset / 2, 
            Layer.Size.ToVector2()/2f, Transform.Scale, Transform.Rotation, Layer.Size.X, Layer.Size.Y);
        return _bounds;
      }
    }
    public override void Render(Batcher batcher, Camera camera) {}
  }

}
