using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven
{
   /// <summary>
  /// The data that holds the information needed to be rendered. Leaf in world hierarcy;  
  /// </summary>
  public class Layer : ICloneable
  {
    /// <summary>
    /// The Level this Layer is attached to
    /// </summary>
    [JsonInclude]
    public Level Level { get; internal set; }

    public World World { get => Level.World; }

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
    public bool IsLocked = false;

    /// <summary>
    /// Position relative to Level
    /// </summary>
    public Vector2 Offset = new Vector2();

    /// <summary>
    /// Boudns soly of this layer with an offset
    /// </summary>
    public RectangleF Bounds { get => Level.Bounds.AddPosition(Offset); }

    /// <summary>
    /// Position of thie layer in world 
    /// </summary>
    public Vector2 Position { get => Level.Bounds.Location + Offset; }

    /// <summary>
    /// Same as Level's size
    /// </summary>
    public Point Size { get => Level.ContentSize; }

    internal Layer()
    {
    }

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

    public bool CanBeEdited() => !IsLocked && IsVisible;

    public virtual void OnLevelResized(Point old) {}

    /// <summary>
    /// Creates a full copy of this Layer
    /// </summary>
    public virtual Layer Copy() => MemberwiseClone() as Layer;

    object ICloneable.Clone() => Copy();
  }

}
