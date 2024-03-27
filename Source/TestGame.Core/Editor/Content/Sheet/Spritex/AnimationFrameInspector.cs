using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;

namespace Raven.Sheet
{
  public class AnimationFrameInspector : Widget.PropertiedWindow
  {
    AnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public AnimationFrameInspector(AnimationEditor animEditor) 
    {
      _animEditor = animEditor;
      NoClose = false;
    } 
    public override void Render(Editor editor)
    {
      if (Animator != null) base.Render(editor);
    }
    protected override void OnRenderAfterName()
    {
    }
  }
}
