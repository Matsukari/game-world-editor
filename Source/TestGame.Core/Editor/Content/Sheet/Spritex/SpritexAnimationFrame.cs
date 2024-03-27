
using Raven.Sheet.Sprites;

namespace Raven.Sheet
{
  // Each for corresponding components
  public class SpritexAnimationFrame : AnimationFrame
  {
    public List<SourcedSprite> Parts = new List<SourcedSprite>();
    public SpritexAnimationFrame(Spritex spritex)
    {
      Parts = spritex.DuplicateParts();
    }

  }

}
