using ImGuiNET;

namespace Raven
{
  public class SpriteAnimationFrameInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Frame.Name; set => Frame.Name = value;}
    public override PropertyList Properties { get => Frame.Properties; set => Frame.Properties = value; }
    public override bool CanOpen => Animator != null;

    SpriteAnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public AnimationFrame Frame { get => _animEditor.SelectedFrame; }

    public SpriteAnimationFrameInspector(SpriteAnimationEditor animEditor) 
    {
      _animEditor = animEditor;
      NoClose = false;
    } 
    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      ImGui.LabelText("Frame", Animator.CurrentIndex.ToString());
      ImGui.InputFloat("Duration", ref Frame.Duration);
      // FramePart.Transform.RenderImGui();
    }
  }
}
