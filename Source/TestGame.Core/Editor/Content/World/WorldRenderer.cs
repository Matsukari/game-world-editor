using Nez;

namespace Raven 
{
  public class WorldRenderer
  { 
    public static void RenderLayer(Batcher batcher, Camera camera, Layer layer)
    {
      if (layer is TileLayer tileLayer)
      {
        TileLayerRenderer.Render(batcher, camera, tileLayer, Transform.Default);
      }
      else if (layer is FreeformLayer freeform)
      {
        if (freeform.IsYSorted) 
        {
          freeform.SortScenes();
        }
        foreach (var spriteScene in freeform.SpriteScenees)
        {
          FreeformLayerRenderer.RenderScene(spriteScene, layer.Bounds.Location, batcher, camera);
        } 
      }
    }
    public static void Render(Batcher batcher, Camera camera, World world) 
    {
      foreach (var level in world.Levels) 
      {
        foreach (var layer in level.Layers)
        {
          RenderLayer(batcher, camera, layer);
        }
      }
    }
  }
}
