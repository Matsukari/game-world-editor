using Nez;
using Microsoft.Xna.Framework;

namespace Raven 
{
  public class WorldRenderer
  { 
    public static void RenderLayer(Batcher batcher, Camera camera, Layer layer, Color color=default, Color nameColor=default)
    {
      if (layer is TileLayer tileLayer)
      {
        TileLayerRenderer.Render(batcher, camera, tileLayer, Transform.Default, color);
      }
      else if (layer is FreeformLayer freeform)
      {
        if (freeform.IsYSorted) 
        {
          freeform.SortScenes();
        }
        foreach (var spriteScene in freeform.SpriteScenees)
        {
          FreeformLayerRenderer.RenderScene(spriteScene, layer.Bounds.Location, batcher, camera, color);
          if (nameColor == default) nameColor = Color.White;
          if (spriteScene.Name != string.Empty)
            batcher.DrawStringCentered(camera, 
                spriteScene.Name, layer.Bounds.Location + spriteScene.ContentBounds.BottomCenter(), 
                nameColor, new Vector2(1, 5), true, true);

        } 
      }
    }
    public static void Render(Batcher batcher, Camera camera, World world) 
    {
      foreach (var level in world.Levels) 
      {
        foreach (var layer in level.Layers)
        {
          if (layer.IsVisible)
            RenderLayer(batcher, camera, layer);
        }
      }
    }
  }
}
