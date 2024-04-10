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
        RenderScene(Layer.SpriteScenees[i].Item1, Transform.Position + LocalOffset + Layer.Bounds.Location, Layer.SpriteScenees[i].Item2, batcher, camera);
      }
    }
    public static void RenderScene(SpriteScene SpriteScene, Vector2 position, RenderProperties props, Batcher batcher, Camera camera)
    {
      foreach (var sprite in SpriteScene.Parts)
      {
        if (!sprite.IsVisible) return;
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: position + sprite.SceneBounds.Location + props.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: sprite.Color.ToColor(),
            rotation: props.Transform.Rotation + SpriteScene.Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: props.Transform.Scale * SpriteScene.Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: 0);
      }
    }    
  }
  /// <summary>
  /// A Layer that can only contain SpriteScenes;
  /// </summary> 
  public class FreeformLayer : Layer
  {
    /// <summary>
    /// Sorts the Renderables based on their Y corrdinates; bottom are in the front, upper is sent to back
    /// </summary>
    class SceneYComparer : IComparer<(SpriteScene, RenderProperties)>
    {
      public int Compare((SpriteScene, RenderProperties) self, (SpriteScene, RenderProperties) other)
      {
        var selfBot = self.Item1.Bounds.Bottom + self.Item2.Transform.Position.Y;
        var otherBot = other.Item1.Bounds.Bottom + other.Item2.Transform.Position.Y;

        var res = otherBot > selfBot ? -1 : otherBot < selfBot ? 1 : 0;
        return res;
      }
    }
    public bool IsYSorted = true;
    public List<(SpriteScene, RenderProperties)> SpriteScenees { get; private set; }= new List<(SpriteScene, RenderProperties)>();

    public FreeformLayer(Level level) : base(level) {}

    public RenderProperties PaintSpriteScene(SpriteScene spriteScene)
    {
      var props = new RenderProperties();
      SpriteScenees.Add((spriteScene, props));
      return props;
    }
    public int GetSceneAt(Vector2 position) => SpriteScenees.FindLastIndex(scene => scene.Item1.Bounds.AddTransform(scene.Item2.Transform).Contains(position));
    public void RemoveSpriteSceneAt(Vector2 position)
    {
      var index = GetSceneAt(position);
      if (index != -1) SpriteScenees.RemoveAt(index);
    }
    public bool GetSceneAt(Vector2 position, out SpriteScene scene, out RenderProperties props) 
    {
      var index = GetSceneAt(position);
      
      if (index != -1) 
      {
        props = SpriteScenees[index].Item2;
        scene = SpriteScenees[index].Item1;
        return true;
      }
      props = null;
      scene = null;
      return false;
    }
    public void SortScenes()
    {
      SpriteScenees.Sort(new SceneYComparer());
    }
  }
}
