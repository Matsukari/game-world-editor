
namespace Raven 
{
  // <summary>
  // Basic implementation of animation frame. You only need to animate the spritexes (with transforms). 
  // Some custom operations, if any, just add some properties
  // </summary>
  public class AnimationFrame : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();
    public string Name = "";
    public float Duration = 100;
  }
}
