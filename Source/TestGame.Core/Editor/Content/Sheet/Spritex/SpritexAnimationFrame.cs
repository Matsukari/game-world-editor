
using Raven.Sheet.Sprites;
using Nez.Tweens;

namespace Raven.Sheet
{
  /// <summary>
  /// This should create a list of state for every parts in the spritex, and should be uniform throughout the 
  /// animation process. This means that any added components to the actual spritex after the creation of animation 
  /// shouldnt be included in this frames'. And any lost components in the atual spritex should be removed from interpolation
  /// </summary>
  public class SpritexAnimationFrame : AnimationFrame
  {
    public List<SourcedSprite> Parts = new List<SourcedSprite>();

    private SpritexAnimationFrame()
    {}
    public SpritexAnimationFrame(Spritex spritex)
    {
      Parts = spritex.DuplicateParts();
    }
    
    /// <summary>
    /// Modify any parts in the actual spritex that collides with any existing parts in this particular frame,
    /// If the part exist in the frame but not in the context; then it was lost outside the animation 
    /// Alternatively, if the part doest exist in the frame but is in the context; then that part was added after 
    /// the creation of the frame, is irrelevant in the time the frame was issued and should be ignored only in this particular frame.
    /// </summary>
    public void Apply(Spritex spritex)
    {  
      var max = Math.Max(spritex.Parts.Count, Parts.Count);
      for (int i = 0; i < max; i++)
      {
        // Fallback
        try 
        {
          spritex.Parts[i] = Parts[i].Duplicate();
        }
        catch (Exception)
        {
        }
      }
    }
    public void BeginReference(Spritex spritex)
    {
      spritex.Parts = Parts;
    }
    public void EndReference(Spritex spritex)
    {
      Parts = spritex.DuplicateParts();
    }
    public override void Interpolate(AnimationFrame prev, object target, float ease)
    {
      var spritex = (Spritex) target;
      var prevFrame = prev as SpritexAnimationFrame;

      for (int i = 0; i < Parts.Count(); i++)
      {       
        try 
        {
          spritex.Parts[i].Transform.Position = Vector2Ext.EaseTo(prevFrame.Parts[i].Transform.Position, Parts[i].Transform.Position, ease);
          spritex.Parts[i].Transform.Scale = Vector2Ext.EaseTo(prevFrame.Parts[i].Transform.Scale, Parts[i].Transform.Scale, ease);
          spritex.Parts[i].Transform.Skew = Vector2Ext.EaseTo(prevFrame.Parts[i].Transform.Skew, Parts[i].Transform.Skew, ease);        
          spritex.Parts[i].Transform.Rotation = prevFrame.Parts[i].Transform.Rotation + ease * (Parts[i].Transform.Rotation - prevFrame.Parts[i].Transform.Rotation);
        }
        // Ignore any parts that doesnt exist in either body
        catch (Exception) 
        {
        }
      }
    }
  }

}
