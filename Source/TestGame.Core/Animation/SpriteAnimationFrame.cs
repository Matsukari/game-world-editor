using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raven 
{
  public class AnimatedSprite : Animation, ISceneSprite
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    public SpriteScene SpriteScene { get; set; }
    public Sprite SourceSprite { get => GetFrame<SpriteAnimationFrame>(CurrentFrame).Sprite; set => throw new Exception(); }

    // Render options 
    public Transform Transform { get; set; } = new Transform(); 
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public Vector2 Origin { get; set; } = new Vector2();

    public Vector4 Color { get; set; } = Vector4.One;

    // Some management options
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; } = false;

    ISceneSprite ISceneSprite.Copy()
    {
      var anim = MemberwiseClone() as AnimatedSprite;
      anim.Properties = Properties.Copy();
      anim.Frames = Frames.CloneItems();  
      anim.Transform = Transform.Duplicate();
      return anim;
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
  public class SpriteAnimationFrame : AnimationFrame
  {
    public Sprite Sprite;
    public SpriteAnimationFrame(Sprite sprite) => Sprite = sprite;

    public override void OnEnter(object target)
    {
      var anim = (Sprite)target;
      anim.Refer(Sprite);
    }
        
    public override AnimationFrame Copy()
    {
      var frame = MemberwiseClone() as SpriteAnimationFrame;
      return frame;
    }

  }
}
