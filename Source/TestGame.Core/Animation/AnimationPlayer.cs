
namespace Raven 
{
  public class AnimationPlayer : Nez.Component
  {  
    public Animation Animation = new Animation();
    public AnimationFrame CurrentFrame { get => Animation.Frames[_currentKeyframe]; }
    public int CurrentIndex { get => _currentKeyframe; }

    public event Action OnKeyframeChanged;
    public event Action OnFinished;

    int _currentKeyframe = 0;
    float _frameTimer = 0;
    bool IsPaused = true;
    bool IsFinished = false;
    bool IsReversed = false;
    bool IsLooping = false;

    public bool Paused;

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
      if (IsPaused && Animation.TotalFrames <= 0) return;

      _frameTimer += dt;
      if (_frameTimer >= Animation.Frames[_currentKeyframe].Duration)
      {
        _frameTimer = 0;
        _currentKeyframe++;
        OnKeyframeChanged();
        if (IsLooping) _currentKeyframe = _currentKeyframe % Animation.TotalFrames;
        else if (_currentKeyframe >= Animation.TotalFrames && !IsFinished) 
        {
          _currentKeyframe--;
          IsFinished = true;
          OnFinished();
        }
      }
    }

  }

}
