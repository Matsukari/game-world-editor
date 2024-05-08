using Nez;
using Microsoft.Xna.Framework;

namespace Raven 
{
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
        Layer.SortScenes();
      }
      for (int i = 0; i < Layer.SpriteScenees.Count; i++)
      {
        RenderScene(Layer.SpriteScenees[i], Transform.Position + LocalOffset + Layer.Bounds.Location, batcher, camera);
      }
    }
    public static void RenderScene(SpriteSceneInstance instance, Vector2 position, Batcher batcher, Camera camera, Color baseColor=default)
    {
      if (baseColor == default) baseColor = Color.White;
      foreach (var sprite in instance.Scene.Parts)
      {
        if (!sprite.IsVisible) return;
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: position + sprite.PlainBounds.AddPosition(sprite.SpriteScene.Transform.Position).Location + instance.Props.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: baseColor.Average(sprite.Color.ToColor()).Average(instance.Props.Color.ToColor()),
            rotation: instance.Props.Transform.Rotation + instance.Scene.Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: instance.Props.Transform.Scale * instance.Scene.Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: 0);
      }
    }    
  }
}
