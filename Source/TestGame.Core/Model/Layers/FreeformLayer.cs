using Microsoft.Xna.Framework;

namespace Raven
{

  /// <summary>
  /// A Layer that can only contain SpriteScenes;
  /// </summary> 
  public class FreeformLayer : Layer
  {
    public bool IsYSorted = true;
    public List<SpriteSceneInstance> SpriteScenees { get; private set; }= new List<SpriteSceneInstance>();

    public FreeformLayer(Level level) : base(level) {}

    public RenderProperties PaintSpriteScene(SpriteScene spriteScene)
    {
      var instance = new SpriteSceneInstance(spriteScene, new RenderProperties());
      SpriteScenees.Add(instance);
      return instance.Props;
    }
    public int GetSceneAt(Vector2 position) => SpriteScenees.FindLastIndex(scene => scene.ContentBounds.AddPosition(Bounds.Location).Contains(position));
    public void RemoveSpriteSceneAt(Vector2 position)
    {
      var index = GetSceneAt(position);
      if (index != -1) SpriteScenees.RemoveAt(index);
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
