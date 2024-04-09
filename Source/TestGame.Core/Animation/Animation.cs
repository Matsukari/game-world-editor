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

    /// <summary>
    /// Identitier of this Animation
    /// </summary>
    public string Name;

    /// <summary>
    /// The context of the play
    /// </summary>
    public object Target;

    /// <summary>
    /// Turning this will make the previous frame of the first frame, last, after it has run once 
    /// </summary>
    public bool IsContinous = false;

    /// <summary>
    /// Linear ordered array of frames
    /// </summary>
    public List<AnimationFrame> Frames = new List<AnimationFrame>();

    /// <summary>
    /// Total ammount of frames
    /// </summary>
    public int TotalFrames { get => Frames.Count(); }

    public Animation Copy()
    {
      var anim = MemberwiseClone() as Animation;
      anim.Name = Name.EnsureNoRepeat();
      anim.Properties = Properties.Copy();
      anim.Frames = Frames.CloneItems();  
      return anim;
    }

    private Animation()
    {
    }
    public Animation(object target, string name = "Animation") 
    {
      Name = name;
      Target = target;
    }
  }
}
