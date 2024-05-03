using Nez;
using Nez.Persistence;
using Microsoft.Xna.Framework;

namespace Raven 
{
  public class SpriteSceneInstance : IPropertied, ICloneable
  {
    [JsonInclude]
    public string Name { get; set; } = "";

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    /// <summary>
    /// The reference to the content, modifying this will modify all instances that depends on this scene
    /// </summary> 
    public SpriteScene Scene { get; internal set; }

    /// <summary>
    /// Options, transform for rendering
    /// </summary> 
    public RenderProperties Props;

    /// <summary>
    /// Lyer this instance is attached on
    /// </summary> 
    public FreeformLayer Layer;

    /// <summary>
    /// This is the bounds of the actual content (sprite parts) 
    /// </summary> 
    public RectangleF ContentBounds { get => Scene.Bounds.AddTransform(Props.Transform).AddPosition(-Scene.MaxOrigin * (Props.Transform.Scale - Vector2.One)); }

    private SpriteSceneInstance()
    {
    }

    public SpriteSceneInstance(SpriteScene scene, RenderProperties props = null)
    {
      Scene = scene;
      if (props == null) props = new RenderProperties();
      Props = props;
    }

    public string GetDimensionId() 
    {
      return Props.Transform.Position.ToString()+Props.Transform.Scale.ToString() + Props.Transform.Scale.ToString();
    }

    object ICloneable.Clone()
    {
      var instance = MemberwiseClone() as SpriteSceneInstance;
      instance.Properties = Properties.Copy();
      instance.Props = Props.Copy();
      return instance;
    }
  }
}
