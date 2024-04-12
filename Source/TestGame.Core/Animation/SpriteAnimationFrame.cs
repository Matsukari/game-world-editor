
namespace Raven 
{
  public class AnimatedSprite : Animation
  {
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
