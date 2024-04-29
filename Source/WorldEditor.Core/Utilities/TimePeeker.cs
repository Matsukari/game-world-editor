using Nez;

namespace Raven.Utils
{
  public struct TimePeeker 
  {
    float _start = 0;
    float _last = 0;
    public TimePeeker() 
    {
      _start = Time.TimeSinceSceneLoad;
      _last = _start;
    }
    public void Peek() 
    {
      _last = Time.TimeSinceSceneLoad;;
    }
    public float SinceLastPeek() 
    {
      return Time.TimeSinceSceneLoad - _last;
    }

  }

    
}
