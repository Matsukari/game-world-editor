using Microsoft.Xna.Framework;
using Nez;

namespace Raven
{
  /// <summary>
  /// Sorts the Renderables based on their Y corrdinates; bottom are in the front, upper is sent to back
  /// </summary>
  class RenderableYComparer : IComparer<IRenderable>
  {
    public int Compare(IRenderable self, IRenderable other)
    {
      var res = other.RenderLayer.CompareTo(self.RenderLayer);
      if (res == 0)
      {
        res = other.Bounds.Bottom > self.Bounds.Bottom ? -1 : other.Bounds.Bottom < self.Bounds.Bottom ? 1 : 0;
        return res;
      }
      return res;
    }
  }
  public class FreeformLayerRenderer : LayerRenderer<FreeformLayer>
  {
    public List<SpriteSceneRenderer> SpriteScenes = new List<SpriteSceneRenderer>();

    /// <summary>
    /// Detaches the given SpriteScene from this Layer and passes it to the World. Call this to 
    /// avoid the Renderable from getting destryoed after Level is destryoed
    /// </summary> 
    public void PassEntityToWorld(SpriteScene spriteScene)
    {
      var index = SpriteScenes.FindIndex(item => item.SpriteScene.Name == spriteScene.Name);
      // SpriteScen doesnt belong to this layer
      if (index != -1) 
        throw new Exception("The given SpriteScene does not exist in the Layer.");

      World.AddComponent(SpriteScenes[index]);
      SpriteScenes.RemoveAt(index);  
    }
    public void PaintSpriteScene(SpriteScene spriteScene)
    {
      Layer.PaintSpriteScene(spriteScene);
      SpriteScenes.Add(new SpriteSceneRenderer(spriteScene));
    }
    public void RemoveSpriteScene(SpriteScene spriteScene)
    {
      var index = SpriteScenes.FindIndex(item => item.SpriteScene.Name == spriteScene.Name);
      if (index == -1) return;
      Layer.RemoveSpriteScene(spriteScene);
      SpriteScenes.RemoveAt(index);
    }
    /// <summary>
    /// Get the topmost SpriteScene that contains the given position
    /// </summary> 
    public SpriteScene GetSpriteSceneAt(Vector2 pos)
    {
      return SpriteScenes.FindLast(item => item.Bounds.Contains(pos))?.SpriteScene;
    }
    public override bool IsVisibleFromCamera(Camera camera)
    {
      return base.IsVisibleFromCamera(camera) && Layer.IsVisible;
    }
    public override void Render(Batcher batcher, Camera camera)
    {
      if (Layer.IsYSorted) 
      {
        SpriteScenes.Sort(new RenderableYComparer());
      }
      foreach (var spriteScene in SpriteScenes)
      {
        spriteScene.Render(batcher, camera);
      }
    }
  }
  /// <summary>
  /// A Layer that can only contain SpriteScenes;
  /// </summary> 
  public class FreeformLayer : Layer
  {
    public bool IsYSorted = true;
    public List<SpriteScene> SpriteScenees = new List<SpriteScene>();

    public FreeformLayer(Level level) : base(level) {}

    public SpriteScene PaintSpriteScene(SpriteScene spriteScene)
    {
      var newSpriteScene = spriteScene.Duplicate(); 
      SpriteScenees.Add(newSpriteScene);
      return newSpriteScene;
    }
    public void RemoveSpriteScene(SpriteScene spriteScene)
    {
      SpriteScenees.Remove(spriteScene);
    }
  }
}
