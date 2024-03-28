using Nez.Tweens;
using Nez.Persistence;


namespace Raven 
{
  /// <summary>
  /// Basic implementation of animation frame. You only need to animate the spritexes (with transforms). 
  /// Some custom operations, if any, just add some properties
  /// </summary>
  public class AnimationFrame : IPropertied
  {
    [JsonExclude]
    string IPropertied.Name { get => Name; set => Name = value; }

    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name = "";

    // The ammmount of time before interpolatin ends
    public float Duration = 1f;

    // Type of ease to be used in interpolaation
    public EaseType EaseType = EaseType.Linear;

    // AnimationPlayer will only interpolate when there is a previous frame in the animtion 
    // Last frame will be the previous frame if IsContinous is set
    public virtual void Interpolate(AnimationFrame prevFrame, object target, float ease) {}

    public virtual void OnEnter(object target) {}
  }
}
