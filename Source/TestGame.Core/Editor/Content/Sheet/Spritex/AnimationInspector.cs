
using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;

namespace Raven.Sheet
{
  public class AnimationInspector : Widget.Window
  {
    AnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public AnimationInspector(AnimationEditor animEditor) 
    {
      Name = GetType().Name;
      _animEditor = animEditor;
      NoClose = false;
    }
    public override void Render(Editor editor)
    {
      if (Animator == null) return;
      base.Render(editor);
    }
    public override void OnRender(Editor editor)
    {
      ImGui.Text("Animation: ");
      ImGui.SameLine();
      ImGui.Text(_animEditor.Animation.Name);

      var frame = (Animator.Animation.TotalFrames > 0) ? Animator.CurrentIndex : 0;
      ImGui.SameLine();
      ImGui.Text("Frame: ");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(48f);
      ImGui.DragInt("##4", ref frame);
      ImGui.BeginChild("animation-content");
      Widget.ImGuiWidget.ButtonSetFlat(_animButtons, 0f);
      ImGui.EndChild();
    } 
    List<(string, Action)> _animButtons = new List<(string, Action)>
    {
      (IconFonts.FontAwesome5.Backward, ()=>{}),
      (IconFonts.FontAwesome5.StepBackward, ()=>{}),
      (IconFonts.FontAwesome5.StepForward, ()=>{}),
      (IconFonts.FontAwesome5.Forward, ()=>{}),
    };
  }
}
