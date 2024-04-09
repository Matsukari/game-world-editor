using Nez.Tweens;
using Nez.Persistence;

namespace Raven 
{
  /// <summary>
  /// Basic implementation of animation frame. You only need to animate the spriteScenees (with transforms). 
  /// Some custom operations, if any, just add some properties
  /// </summary>
  public class AnimationFrame : IPropertied, ICloneable
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();


    public string Name = "";

    /// <summary>
    /// The ammount of time this frame interpolates towards the next frame
    /// </summary>
    public float Duration = 1f;

    /// <summary>
    /// Type of ease to be used to interpolate the frame
    /// </summary>
    public EaseType EaseType = EaseType.Linear;

    /// <summary>
    /// AnimationPlayer will only interpolate when there is a previous frame in the animtion 
    /// Last frame will be the previous frame if IsContinous is set
    /// </summary>
    public virtual void Interpolate(AnimationFrame prevFrame, object target, float ease) {}

    public virtual void OnEnter(object target) {}

    public virtual AnimationFrame Copy() 
    {
      var frame = MemberwiseClone() as AnimationFrame;
      frame.Properties = Properties.Copy();
      return frame;
    }

    object ICloneable.Clone() => Copy();
  }
}
