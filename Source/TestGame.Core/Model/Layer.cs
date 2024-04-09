using Microsoft.Xna.Framework;
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
  /// <summary>
  /// The data that holds the information needed to be rendered. Leaf in world hierarcy;  
  /// </summary>
  public class Layer : ICloneable
  {
    /// <summary>
    /// The Level this Layer is attached to
    /// </summary>
    public Level Level { get; internal set; }

    /// <summary>
    /// The ID of this Layer, cannot be the same with other Layers on the Level
    /// </summary>
    public string Name = "Layer";

    /// <summary>
    /// Modifying this field should affects all renderables about to be rendered 
    /// by a LayerRenderer
    /// </summary>
    public float Opacity = 1f;

    /// <summary>
    /// Field to indicate if this Layer is to be rendered
    /// </summary>
    public bool IsVisible = true;

    /// <summary>
    /// Field to indicate if this Layer can accept any modifications
    /// </summary>
    public bool IsLocked = true;

    /// <summary>
    /// Position relative to Level
    /// </summary>
    public Vector2 Offset = new Vector2();

    /// <summary>
    /// Boudns soly of this layer with an offset
    /// </summary>
    public RectangleF Bounds { get => Level.Bounds.AddPosition(Offset); }

    /// <summary>
    /// Same as Level's size
    /// </summary>
    public Point Size { get => Level.ContentSize; }

    public Layer(Level level)
    {
      Level = level;
      Name = GetType().Name;
    }
 
    /// <summary>
    /// Remoes this layer from its parent World; the World can no longer refenrece this level but 
    /// this level could in reverse
    /// </summary>
    public void DetachFromLevel() => Level.RemoveLayer(Name);

    /// <summary>
    /// Creates a full copy of this Layer
    /// </summary>
    public virtual Layer Copy() => MemberwiseClone() as Layer;

    object ICloneable.Clone() => Copy();
  }

}
