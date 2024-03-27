

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
    public List<AnimationFrame> Frames = new List<AnimationFrame>();
    public int TotalFrames { get => Frames.Count(); }
    public Animation(string name = "") => Name = name;
  }
}
