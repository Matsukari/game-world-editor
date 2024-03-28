

namespace Raven 
{
  // <summary>
  // Basic implementation of animation frame. You only need to animate the spritexes (with transforms). 
  // Some custom operations, if any, just add some properties
  // </summary>
  public class Animation : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name;

    // The context of the play
    public object Target;

    // This will make the previous frame of the first frame after it has run once 
    public bool IsContinous = false;

    public List<AnimationFrame> Frames = new List<AnimationFrame>();
    public int TotalFrames { get => Frames.Count(); }


    public Animation(object target, string name = "") 
    {
      Name = name;
      Target = target;
    }
  }
}
