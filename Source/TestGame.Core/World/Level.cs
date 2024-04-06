using Microsoft.Xna.Framework;
using Nez;

namespace Raven
{
  public class LevelEntity : Entity
  {
    public readonly Level Level;
    public WorldEntity WorldEntity { get => Parent.Entity as WorldEntity; }

    public RectangleF Bounds 
    { 
      get 
      {
        _bounds.CalculateBounds(Transform.Position + Level.Bounds.Location, Vector2.Zero, 
            Level.ContentSize.ToVector2()/2f, Transform.Scale, Transform.Rotation, Level.ContentSize.X, Level.ContentSize.Y);
        return _bounds;
      }
    }

    RectangleF _bounds = new RectangleF();

    // Only created in WorldEntity
    internal LevelEntity(Level level)
    {
      Level = level;
    }

    public void AddLayer(Layer layer) 
    {
      Level.AddLayer(layer);

      if (layer is TileLayer tiled) 
        AddComponent(new TileLayerRenderer()).Initialize(tiled);

      else if (layer is FreeformLayer freeform) 
        AddComponent(new FreeformLayerRenderer()).Initialize(freeform);

      else 
        throw new Exception($"Layer of type {layer.GetType().Name} cannot be Added");

    }
    public void RemoveLayer(Layer layer)
    {
      Level.RemoveLayer(layer.Name);
      for (int i = 0; i < Components.Count; i++)
      {
        if ((Components[i] is TileLayerRenderer tileLayerRenderer && tileLayerRenderer.Layer.Name == layer.Name) 
          || Components[i] is FreeformLayerRenderer freeformLayerRenderer && freeformLayerRenderer.Layer.Name == layer.Name)
          RemoveComponent(Components[i]);
      }
    }
    public void RemoveFromWorld()
    {
      WorldEntity.RemoveLevel(Level);
    }
  }

  /// <summary>
  /// Composed of Layers of the same size as this.
  /// </summary>
  public class Level : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name;

    /// <summary>
    /// The size of the Level, inherited by all owned Layers
    /// </summary>
    public Point ContentSize = new Point(Screen.Width, Screen.Height);

    /// <summary>
    /// World where this Level is attached
    /// </summary>
    public readonly World World;

    /// <summary>
    /// Actual content
    /// </summary>
    public List<Layer> Layers = new List<Layer>();

    /// <summary>
    /// Position offset from World
    /// </summary>
    public Vector2 LocalOffset = Vector2.Zero;

    /// <summary>
    /// Determines whether to draw this Level or not
    /// </summary>
    public bool IsVisible = true;

    /// <summary>
    /// Absolute bounds of the Level
    /// </summary>
    public RectangleF Bounds { get => new RectangleF(
        World.Position.X + LocalOffset.X, 
        World.Position.Y + LocalOffset.Y,
        ContentSize.X,
        ContentSize.Y);
    }


    public Level(World world)
    {
      World = world;
    }

    /// <summary>
    /// Adds a layer at the forefront
    /// </summary>
    public void AddLayer(Layer layer) => Layers.Add(layer);

    /// <summary>
    /// Remoes the layer with the given name
    /// </summary>
    public void RemoveLayer(string name) => Layers.Remove(Layers.Find(item => item.Name == name));

    /// <summary>
    /// Remoes this layer from its parent World; the World can no longer refenrece this level but 
    /// this level could in reverse
    /// </summary>
    public void DetachFromWorld() => World.RemoveLevel(this);

  }
}
