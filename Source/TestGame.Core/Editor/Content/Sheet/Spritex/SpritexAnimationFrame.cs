
using Raven.Sheet.Sprites;
using Nez.Tweens;

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
    public override void OnEnter(object target)
    {

    }  
    public override void Interpolate(AnimationFrame prev, object target, float ease)
    {
      var spritex = (Spritex) target;
      var prevFrame = prev as SpritexAnimationFrame;

      for (int i = 0; i < Parts.Count(); i++)
      {        
        spritex.Parts[i].Transform.Position = Vector2Ext.EaseTo(prevFrame.Parts[i].Transform.Position, Parts[i].Transform.Position, ease);
        spritex.Parts[i].Transform.Scale = Vector2Ext.EaseTo(prevFrame.Parts[i].Transform.Scale, Parts[i].Transform.Scale, ease);
        spritex.Parts[i].Transform.Skew = Vector2Ext.EaseTo(prevFrame.Parts[i].Transform.Skew, Parts[i].Transform.Skew, ease);        
        spritex.Parts[i].Transform.Rotation = prevFrame.Parts[i].Transform.Rotation + ease * (Parts[i].Transform.Rotation - prevFrame.Parts[i].Transform.Rotation);
      }
    }
  }

}
