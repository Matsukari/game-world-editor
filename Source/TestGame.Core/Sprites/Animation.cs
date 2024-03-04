

namespace Raven.Sheet.Sprites
{
  public class Frame : Propertied 
  {
    private List<int> KeyFrames;
    public float Fps;
  }
  public class KeyFrame : Propertied
  {
    public float Time;
    private List<Transform> Frames;
  }
  public class Animation 
  {
    List<object> _data = new List<object>();

  }
}
