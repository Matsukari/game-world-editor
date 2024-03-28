
namespace Raven 
{
  public class AnimationPlayer : Nez.Component, Nez.IUpdatable
  {  
    public Animation Animation;
    public AnimationFrame CurrentFrame { get => (Animation.TotalFrames > 0) ? Animation.Frames[CurrentIndex] : null; }
    public int CurrentIndex { get => _currentKeyframe; }
    public int Delta { get => (IsReversed) ? -1 : 1; }

    public event Action OnKeyframeChanged;

    // Only called once every after call in Start()
    public event Action OnFinished;

    int _currentKeyframe = 0;
    public float _frameTimer = 0;
    public bool IsPaused = true;
    public bool IsFinished = false;
    public bool IsReversed = false;
    public bool IsLooping = true;
    public bool Paused = true;

    public void Reset() 
    {
      IsFinished = false;
      IsPaused = false;
      _frameTimer = 0;
      _currentKeyframe = 0;
    }
    public void TooglePlay() 
    {
      IsPaused = !IsPaused;
      IsFinished = false;
    }
    public void JumpTo(int index, bool resetTime=true)
    {
      _currentKeyframe = Math.Max(0, index);
      if (resetTime) _frameTimer = 0;

      if (IsLooping) _currentKeyframe = _currentKeyframe % Animation.TotalFrames;
      // Never executes if IsLooping is enabled 
      if (_currentKeyframe >= Animation.TotalFrames && !IsFinished) 
      {
        _currentKeyframe = Animation.TotalFrames - 1;
        Paused = true;
        IsFinished = true;
        if (OnFinished != null) OnFinished();
      }
      CurrentFrame.OnEnter(Animation.Target);

      _currentKeyframe = Math.Min(_currentKeyframe, Animation.TotalFrames-1);
    }
    public void Backward() => JumpTo(_currentKeyframe + Delta * -1);
    public void Forward() => JumpTo(_currentKeyframe + Delta * 1);
    public void Update() => Update(Nez.Time.DeltaTime);
    public void Update(float dt)
    {
      if (!IsRunning()) return;

      _frameTimer += dt;
      Console.WriteLine($"timer: {_frameTimer}, index: {CurrentIndex}");

      // Preps
      var time = Math.Min(_frameTimer / CurrentFrame.Duration, 1f);
      var ease = Nez.Tweens.EaseHelper.Ease(CurrentFrame.EaseType, time, CurrentFrame.Duration); 

      // Interpolate 
      var prevIndex = _currentKeyframe + Delta * -1;
      if (prevIndex >= 0 && prevIndex < Animation.TotalFrames)
        CurrentFrame.Interpolate(Animation.Frames[prevIndex], Animation.Target, ease);

      // Ends frame
      if (_frameTimer >= CurrentFrame.Duration)
      {
        Forward();
        if (OnKeyframeChanged != null) OnKeyframeChanged();
      }
    }
    public bool IsRunning() => !(
        IsPaused
        || IsFinished
        || Animation == null 
        || Animation.TotalFrames <= 0 
        || Animation.Target == null); 
  }

}
