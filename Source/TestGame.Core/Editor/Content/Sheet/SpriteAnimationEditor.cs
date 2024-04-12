
namespace Raven
{
  public class SpriteAnimationEditor : EditorInterface, IImGuiRenderable
  {
    SpriteAnimationFrameInspector _frameInspector;
    SpriteAnimationInspector _inspector;
    AnimationPlayer _player;
    public readonly Sheet Sheet;
    public Animation Animation;
    public AnimationPlayer Player { get => _player; }
    public bool IsOpen { get => _player != null && _inspector.IsOpen; }
    public SpriteAnimationFrame SelectedFrame;

    public event Action OnClose;
    public event Action<int, int> OnSelectFrame;

    public SpriteAnimationEditor(Sheet sheet)
    {
      Sheet = sheet;
      _frameInspector = new SpriteAnimationFrameInspector(this);
      _inspector = new SpriteAnimationInspector(this);
    }
    public void SelectFrame(int index)
    {
      // Editor.GetEditorComponent<Selection>().End();
      _frameInspector.IsOpen = true;
      _player.JumpTo(index);
      var newFrame = Animation.Frames[index] as SpriteAnimationFrame;
      SelectedFrame = newFrame;
    }
    public void Open(Animation animation)
    {
      Animation = animation;
      _inspector.IsOpen = true;
      _player = new AnimationPlayer();
      _player.Animation = Animation;
    }
    public void Close() 
    {
      _player = null;
      if (OnClose != null) OnClose(); 
    }
    void IImGuiRenderable.Render(Raven.ImGuiWinManager imgui)
    {
      _inspector.Animator = _player;
      _inspector.Render(imgui);  

      if (SelectedFrame != null && _inspector.CanOpen)
      {
        _frameInspector.Animator = _player;
        _frameInspector.Render(imgui);
      }

      if (_player != null) _player.Update();
    } 
  }
}
