using ImGuiNET;

namespace Raven
{
  public class SpriteAnimationFrameInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Frame.Name; set => Frame.Name = value;}
    public override PropertyList Properties { get => Frame.Properties; set => Frame.Properties = value; }

    SpriteAnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public AnimationFrame Frame { get => _animEditor.SelectedFrame; }

    public SpriteAnimationFrameInspector(SpriteAnimationEditor animEditor) 
    {
      _animEditor = animEditor;
      NoClose = false;
    } 
    public override void Render(ImGuiWinManager imgui)
    {
      if (Animator != null) base.Render(imgui);
    }
    protected override void OnRenderAfterName()
    {
      ImGui.LabelText("Frame", Animator.CurrentIndex.ToString());
      ImGui.InputFloat("Duration", ref Frame.Duration);
      // FramePart.Transform.RenderImGui();
    }
  }
}
