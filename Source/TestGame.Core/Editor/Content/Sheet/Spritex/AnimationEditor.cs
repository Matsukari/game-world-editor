using Microsoft.Xna.Framework;
using ImGuiNET;
using Raven.Sheet.Sprites;

namespace Raven.Sheet
{
  public class AnimationEditor : EditorComponent, IImGuiRenderable
  {
    AnimationFrameInspector _frameInspector;
    AnimationInspector _inspector;
    AnimationPlayer _player;
    public Animation Animation;
    public Spritex Spritex;
    public bool IsOpen { get => _player != null; }
    public SpritexAnimationFrame SelectedFrame;

    public AnimationEditor()
    {
      _frameInspector = new AnimationFrameInspector(this);
      _inspector = new AnimationInspector(this);
    }
    public void Open(Spritex spritex, Animation animation)
    {
      Spritex = spritex;
      Animation = animation;
      _player = new AnimationPlayer();
      _player.Animation = Animation;
    }
    public void Close() 
    {
      _player = null;
    }
    public void AddFrameFromCurrentState()
    {
      if (!IsOpen) return;
      var frame = new SpritexAnimationFrame(Spritex);
      var index = _player.CurrentIndex;
      Spritex.InsertFrame(Animation.Name, index, frame);
    }
    public override void OnContent()
    {
      RestrictTo<Sheet>();
    }
    public void Render(Editor editor)
    {
      _inspector.Animator = _player;
      _inspector.Render(editor);  

      if (SelectedFrame != null)
      {
        _frameInspector.Animator = _player;
        _frameInspector.Render(editor);
      }

      if (_player != null) _player.Update();
    } 
  }
}
