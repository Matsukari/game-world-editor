
namespace Raven 
{
  public class SpriteAnimationFrame : AnimationFrame
  {
    public Sprite Sprite;

    private SpriteAnimationFrame() {}

    public SpriteAnimationFrame(Sprite sprite) => Sprite = sprite;

    public override void OnEnter(object target)
    {
      // var anim = (Sprite)target;
      // anim.Refer(Sprite);
    }
        
    public override AnimationFrame Copy()
    {
      var frame = MemberwiseClone() as SpriteAnimationFrame;
      return frame;
    }

  }
}
