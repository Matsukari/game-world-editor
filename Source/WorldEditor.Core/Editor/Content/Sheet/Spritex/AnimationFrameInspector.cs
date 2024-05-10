using ImGuiNET;

namespace Raven
{
  public class AnimationFrameInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Frame.Name; set => Frame.Name = value;}
    public override PropertyList Properties { get => Frame.Properties; set => Frame.Properties = value; }
    public override bool CanOpen => Animator != null;

    AnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public SpriteSceneAnimationFrame Frame { get => _animEditor.SelectedFrame; }
    public ISceneSprite FramePart { get => _animEditor.SelectedFramePart; }

    public AnimationFrameInspector(AnimationEditor animEditor) 
    {
      _animEditor = animEditor;
      NoClose = false;
    } 
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (ImGui.CollapsingHeader("AnimationFrame", ImGuiTreeNodeFlags.DefaultOpen))
      {
        NameInput();
        ImGui.LabelText("Frame", Animator.CurrentIndex.ToString());
        ImGui.InputFloat("Duration", ref Frame.Duration);
        SpritePartInspector.RenderSprite(imgui, FramePart, false);
        EaseTypePicker.Picker(ref Frame.EaseType);
      }
      PropertiesRenderer.Render(imgui, this, OnChangeProperty);
    }
  }
}
