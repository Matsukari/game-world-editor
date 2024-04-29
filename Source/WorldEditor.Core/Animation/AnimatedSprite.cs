using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Persistence;

namespace Raven
{
  public class AnimatedSprite : Animation, ISceneSprite
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    public SpriteScene SpriteScene { get; set; }

    [JsonExclude]
    public Sprite SourceSprite { get => GetFrame<SpriteAnimationFrame>(CurrentFrame).Sprite; set => (Frames[CurrentFrame] as SpriteAnimationFrame).Sprite = value; }

    // Render options 
    [JsonInclude]
    public Transform Transform { get; set; } = new Transform(); 

    [JsonInclude]
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    [JsonInclude]
    public Vector2 Origin { get; set; } = new Vector2();

    [JsonInclude]
    public Vector4 Color { get; set; } = Vector4.One;

    [JsonInclude]
    public bool IsVisible { get; set; } = true;

    [JsonInclude]
    public bool IsLocked { get; set; } = false;

    ISceneSprite ISceneSprite.Copy()
    {
      var anim = MemberwiseClone() as AnimatedSprite;
      anim.Properties = Properties.Copy();
      anim.Frames = Frames.CloneItems();  
      anim.Transform = Transform.Duplicate();
      anim.Target = Target;
      return anim;
    }

    private AnimatedSprite() 
    {
    }

    public AnimatedSprite(List<Sprite> frames, float dur=0.1f) : base(new Sprite(null))
    {
      foreach (var frame in frames) 
      {
        var f = new SpriteAnimationFrame(frame);
        f.Duration = dur;
        f.Properties = frame.Properties;
        f.Name = frame.Name;
        Frames.Add(f);
      }
    }
  }

}
