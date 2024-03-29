using Nez.Persistence;

namespace Raven 
{
  /// <summary>
  /// Manages AnimationFrames. Set a Target for a context which would be called along when performing operations on a frame
  /// </summary>
  public class Animation : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name;

    // The context of the play
    public object Target;

    // This will make the previous frame of the first frame after it has run once 
    public bool IsContinous = false;

    public List<AnimationFrame> Frames = new List<AnimationFrame>();

    public int TotalFrames { get => Frames.Count(); }

    private Animation()
    {
    }
    public Animation(object target, string name = "") 
    {
      Name = name;
      Target = target;
    }
  }
}
