using Nez;

namespace Raven.Utils
{
  public struct Timer 
  {
    public float EverySecond = 0;
    public bool HasStarted = false;
    private TimePeeker _peeker = new TimePeeker();
    public Timer(float EverySecond) { this.EverySecond = EverySecond; }
    public void start() 
    {
      HasStarted = true;
      _peeker.Peek();
    }
    public bool IsFinished() 
    {
      return _peeker.SinceLastPeek() > EverySecond && HasStarted;
    }
    public void stop() 
    {
      HasStarted = false;
    }
    public void reset() 
    {
      _peeker.Peek();
    }
    public float normalize() 
    {
      if (!HasStarted) return 0;
      return Math.Min(_peeker.SinceLastPeek() / EverySecond, 1f);
    }
    public bool IsTicking() 
    {
      return HasStarted;
    }
  }
}
