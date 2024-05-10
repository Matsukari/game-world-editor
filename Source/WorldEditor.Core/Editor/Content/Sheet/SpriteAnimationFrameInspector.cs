using ImGuiNET;
using Nez.Tweens;

namespace Raven
{
  public static class EaseTypePicker
  {
    static List<string> _types = new List<string>();

    public static void Picker(ref EaseType ease)
    {
      if (_types.Count() == 0)
      {
        foreach (var type in Enum.GetNames<EaseType>())
        {
          _types.Add(type);
        }
      }
      var e = ease;
      var i = _types.FindIndex(item => item == Enum.GetName<EaseType>(e));
      if (ImGui.Combo("Ease Type", ref i, _types.ToArray(), _types.Count())) ease = Enum.GetValues<EaseType>()[i];
    }
  }
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
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (ImGui.CollapsingHeader("AnimationFrame", ImGuiTreeNodeFlags.DefaultOpen))
      {
        NameInput();
        ImGui.LabelText("Frame", Animator.CurrentIndex.ToString());
        ImGui.InputFloat("Duration", ref Frame.Duration);
        EaseTypePicker.Picker(ref Frame.EaseType);
      }
      PropertiesRenderer.Render(imgui, this, OnChangeProperty);
    }
  }
}
