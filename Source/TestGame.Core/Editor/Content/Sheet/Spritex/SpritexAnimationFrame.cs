
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
    public void Apply(Spritex spritex)
    {  
      spritex.Parts.Clear();
      var list = new List<SourcedSprite>();
      foreach (var part in Parts) list.Add(part.Duplicate());
      spritex.Parts = list;
    }
    public void BeginReference(Spritex spritex)
    {
      spritex.Parts = Parts;
    }
    public void EndReference(Spritex spritex)
    {
      Parts = spritex.DuplicateParts();
    }
  }

}
