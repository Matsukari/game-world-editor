
namespace Raven 
{
  public class AnimationPlayer : Nez.Component
  {  
    public Animation Animation = new Animation();
    public AnimationFrame CurrentFrame { get => Animation.Frames[_currentKeyframe]; }
    public int CurrentIndex { 
      get => _currentKeyframe; 
      set
      {
        _currentKeyframe = Math.Max(value, 0);
        if (IsLooping) 
        {
          _currentKeyframe = _currentKeyframe % Animation.TotalFrames;
          return;
        } 
        if (_currentKeyframe >= Animation.TotalFrames && !IsFinished) 
        {
          _currentKeyframe--;
          IsFinished = true;
          if (OnFinished != null) OnFinished();
        }
        _currentKeyframe = Math.Min(_currentKeyframe, Animation.TotalFrames-1);
      }
    }

    public event Action OnKeyframeChanged;
    public event Action OnFinished;

    int _currentKeyframe = 0;
    public float _frameTimer = 0;
    public bool IsPaused = true;
    public bool IsFinished = false;
    public bool IsReversed = false;
    public bool IsLooping = false;

    public bool Paused = true;

    public void Start() 
    {
      IsFinished = false;
      IsPaused = false;
      _frameTimer = 0;
      _currentKeyframe = 0;
    }
    public void TooglePlay() 
    {
      IsPaused = !IsPaused;
    }
    public void Update() => Update(Nez.Time.DeltaTime);
    public void Update(float dt)
    {
      if (IsPaused || Animation.TotalFrames <= 0) return;

      _frameTimer += dt;
      if (_frameTimer >= Animation.Frames[_currentKeyframe].Duration)
      {
        _frameTimer = 0;
        CurrentIndex++;
        if (OnKeyframeChanged != null) OnKeyframeChanged();
      }
    }
  }

}
