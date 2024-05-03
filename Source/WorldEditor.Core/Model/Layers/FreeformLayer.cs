using Microsoft.Xna.Framework;
using Nez.Persistence;

namespace Raven
{

  /// <summary>
  /// A Layer that can only contain SpriteScenes;
  /// </summary> 
  public class FreeformLayer : Layer
  {
    public bool IsYSorted = true;

    [JsonInclude]
    public List<SpriteSceneInstance> SpriteScenees { get; private set; }= new List<SpriteSceneInstance>();

    internal FreeformLayer()
    {
    }

    public FreeformLayer(Level level) : base(level) {}

    public SpriteSceneInstance PaintSpriteScene(SpriteScene spriteScene)
    {
      var instance = new SpriteSceneInstance(spriteScene, new RenderProperties());
      instance.Layer = this;
      SpriteScenees.Add(instance);
      return instance;
    }
    public int GetSceneAt(Vector2 position) => SpriteScenees.FindLastIndex(scene => scene.ContentBounds.AddPosition(Bounds.Location).Contains(position));

    public SpriteSceneInstance RemoveSpriteSceneAt(Vector2 position)
    {
      var index = GetSceneAt(position);
      if (index != -1) 
      {
        var instance = SpriteScenees[index];
        instance.Layer = null;
        SpriteScenees.RemoveAt(index);
        return instance;
      }
      return null;
    }
    public bool GetSceneAt(Vector2 position, out SpriteSceneInstance instance) 
    {
      var index = GetSceneAt(position);
      
      if (index != -1) 
      {
        instance = SpriteScenees[index];
        return true;
      }
      instance = null;
      return false;
    }
    public void SortScenes()
    {
      SpriteScenees.Sort(new SceneYComparer());
    }
    public override Layer Copy()
    {
      var layer = MemberwiseClone() as FreeformLayer;
      layer.SpriteScenees = SpriteScenees.CloneItems();
      return layer;

    }
      
  }
}
