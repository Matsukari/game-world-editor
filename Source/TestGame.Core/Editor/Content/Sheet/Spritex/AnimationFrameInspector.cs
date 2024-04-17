using ImGuiNET;

namespace Raven
{
  public class AnimationFrameInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Frame.Name; set => Frame.Name = value;}
    public override PropertyList Properties { get => Frame.Properties; set => Frame.Properties = value; }

    AnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public SpriteSceneAnimationFrame Frame { get => _animEditor.SelectedFrame; }
    public SourcedSprite  FramePart { get => _animEditor.SelectedFramePart; }

    public AnimationFrameInspector(AnimationEditor animEditor) 
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
      SpriteSceneInspector.RenderSprite(ImGuiManager, FramePart, false);
    }
  }
}
