using Microsoft.Xna.Framework;
using ImGuiNET;
using Raven.Sheet.Sprites;

namespace Raven
{
  public class AnimationEditor : EditorComponent, IImGuiRenderable
  {
    AnimationFrameInspector _frameInspector;
    AnimationInspector _inspector;
    AnimationPlayer _player;
    public Animation Animation;
    public SpriteScene SpriteScene;
    public bool IsOpen { get => _player != null; }
    public SpriteSceneAnimationFrame SelectedFrame;
    public SourcedSprite SelectedFramePart;

    public AnimationEditor()
    {
      _frameInspector = new AnimationFrameInspector(this);
      _inspector = new AnimationInspector(this);
    }
    public void SelectFrame(int index, int part)
    {
      Editor.GetEditorComponent<Selection>().End();
      _frameInspector.IsOpen = true;
      _player.JumpTo(index);
      var newFrame = Animation.Frames[index] as SpriteSceneAnimationFrame;
      newFrame.Apply(SpriteScene);
      SelectedFrame = newFrame;
      SelectedFramePart = SelectedFrame.Parts[part];
    }
    public void Open(SpriteScene spriteScene, Animation animation)
    {
      Enabled = true;
      SpriteScene = spriteScene;
      Animation = animation;
      _inspector.IsOpen = true;
      _player = new AnimationPlayer();
      _player.Animation = Animation;
    }
    public void Close() 
    {
      Enabled = false;
      _player = null;
    }
    public void AddFrameFromCurrentState()
    {
      if (!IsOpen) return;
      var frame = new SpriteSceneAnimationFrame(SpriteScene);
      var index = _player.CurrentIndex;
      SpriteScene.InsertFrame(Animation.Name, index, frame);
      _player.Forward();
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
