using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;
using Raven.Sheet.Sprites;

namespace Raven.Sheet
{
  public class AnimationFrameInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Frame.Name; set => Frame.Name = value;}
    public override PropertyList Properties { get => Frame.Properties; set => Frame.Properties = value; }

    AnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public SpritexAnimationFrame Frame { get => _animEditor.SelectedFrame; }
    public SourcedSprite  FramePart { get => _animEditor.SelectedFramePart; }

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
      ImGui.LabelText("Frame", Animator.CurrentIndex.ToString());
      ImGui.InputFloat("Duration", ref Frame.Duration);
      // FramePart.Transform.RenderImGui();
      SpritexInspector.RenderSprite(FramePart, false);
    }
  }
}
