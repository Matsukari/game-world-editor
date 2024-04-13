using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

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
        RenderScene(Layer.SpriteScenees[i], Transform.Position + LocalOffset + Layer.Bounds.Location, batcher, camera);
      }
    }
    public static void RenderScene(SpriteSceneInstance instance, Vector2 position, Batcher batcher, Camera camera)
    {
      foreach (var sprite in instance.Scene.Parts)
      {
        if (!sprite.IsVisible) return;
        batcher.Draw(
            texture: sprite.SourceSprite.Texture,
            position: position + sprite.PlainBounds.AddPosition(sprite.SpriteScene.Transform.Position).Location + instance.Props.Transform.Position,
            sourceRectangle: sprite.SourceSprite.Region,
            color: sprite.Color.ToColor(),
            rotation: instance.Props.Transform.Rotation + instance.Scene.Transform.Rotation + sprite.Transform.Rotation,
            origin: sprite.Origin,
            scale: instance.Props.Transform.Scale * instance.Scene.Transform.Scale * sprite.Transform.Scale,
            effects: sprite.SpriteEffects,
            layerDepth: 0);
      }
    }    
  }

  public class SpriteSceneInstance : IPropertied, ICloneable
  {
    [JsonInclude]
    public string Name { get; set; } = "";

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    /// <summary>
    /// The reference to the content, modifying this will modify all instances that depends on this scene
    /// </summary> 
    public readonly SpriteScene Scene;

    /// <summary>
    /// Options, transform for rendering
    /// </summary> 
    public RenderProperties Props;

    /// <summary>
    /// This is the bounds of the actual content (sprite parts) 
    /// </summary> 
    public RectangleF ContentBounds { get => Scene.Bounds.AddTransform(Props.Transform).AddPosition(-Scene.MaxOrigin * (Props.Transform.Scale - Vector2.One)); }

    public SpriteSceneInstance(SpriteScene scene, RenderProperties props = null)
    {
      Scene = scene;
      if (props != null) Props = props;
    }

    object ICloneable.Clone()
    {
      var instance = MemberwiseClone() as SpriteSceneInstance;
      Console.WriteLine("New props");
      instance.Properties = Properties.Copy();
      instance.Props = Props.Copy();
      return instance;
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
    class SceneYComparer : IComparer<SpriteSceneInstance>
    {
      public int Compare(SpriteSceneInstance self, SpriteSceneInstance other)
      {
        var selfBot = self.Props.Transform.Position.Y;
        var otherBot = other.Props.Transform.Position.Y;

        var res = otherBot > selfBot ? -1 : otherBot < selfBot ? 1 : 0;
        return res;
      }
    }
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
