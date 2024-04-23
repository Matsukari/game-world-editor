using Nez;
using Microsoft.Xna.Framework;

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

}
