
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
    Vector2 _trackContentMin = Vector2.Zero;
    public override void OnRender(Editor editor)
    {
      ImGui.Text("Animation: ");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      ImGui.InputText("##2", ref _animEditor.Animation.Name, 20);

      var currFrame = (Animator.Animation.TotalFrames > 0) ? Animator.CurrentIndex : 0;
      ImGui.SameLine();
      ImGui.Text("Frame: ");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(48f);
      if (ImGui.DragInt("##4", ref currFrame) && currFrame >= 0 && currFrame < Animator.Animation.TotalFrames) Animator.CurrentIndex = currFrame;

      Widget.ImGuiWidget.SpanX(10f);
      Widget.ImGuiWidget.ButtonSetFlat(0f,
        (IconFonts.FontAwesome5.Backward,       ()=>Animator.IsReversed = true),
        (IconFonts.FontAwesome5.StepBackward,   ()=>Animator.CurrentIndex--),
        (GetPlayString(Animator),               ()=>Animator.TooglePlay()),
        (IconFonts.FontAwesome5.StepForward,    ()=>Animator.CurrentIndex++),
        (IconFonts.FontAwesome5.Forward,        ()=>Animator.IsReversed = false)
      );      
      Widget.ImGuiWidget.SpanX(10f);
      Widget.ImGuiWidget.DelegateToggleButton(IconFonts.FontAwesome5.SyncAlt, ()=>Animator.IsLooping=!Animator.IsLooping);
      Widget.ImGuiWidget.SpanX(10f);
      Widget.ImGuiWidget.DelegateButton(IconFonts.FontAwesome5.Key, ()=>_animEditor.AddFrameFromCurrentState());

      ImGui.BeginChild("animation-content");
      _trackContentMin = ImGui.GetItemRectMax();
      var childSize = ImGui.GetWindowSize() - _trackContentMin;

      if (Animator.Animation.TotalFrames > 0)
        DrawComponentsTrack();
      else 
      {
        var msg = "This animation contains no frames yet. ";
        var msgSize = ImGui.CalcTextSize(msg);
        ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowWidth()/2-msgSize.X/2, childSize.Y/2-msgSize.Y/2));
        ImGui.SameLine();
        ImGui.TextDisabled(msg);
      }

      ImGui.EndChild();
   }
    bool _isOpenFrameOptions = false;
    SpritexAnimationFrame _frameOnOpenOptions = null;
    void DrawComponentsTrack()
    {
      ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(2, 2));
      if (ImGui.BeginTable("##split", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.Resizable | ImGuiTableFlags.ScrollY))
      {
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableSetupColumn("components");
        ImGui.TableSetupColumn("frames");
        // ImGui.TableHeadersRow();
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(1);
        DrawFrameHeader();

        // Draw components names in left side
        for (int i = 0; i < _animEditor.Spritex.Parts.Count(); i++)
        {
          var part = _animEditor.Spritex.Parts[i];
          ImGui.TableNextRow();
          ImGui.TableSetColumnIndex(0);
          ImGui.Text(part.Name);
          ImGui.TableSetColumnIndex(1);
          // Draw frames of each components in right side
          for (int j = 0; j < Animator.Animation.TotalFrames; j++)
          {
            var frameSet = Animator.Animation.Frames[j] as SpritexAnimationFrame;
            var frame = frameSet.Parts[i];

            var color = (Animator.CurrentIndex == j) ? _animEditor.Editor.Settings.Colors.FrameActive : _animEditor.Editor.Settings.Colors.FrameInactive;
            var frameIcon = IconFonts.FontAwesome5.Circle;
            if (_modifiedFrame != -1) frameIcon = IconFonts.FontAwesome5.ExclamationCircle;
            if (Animator.CurrentIndex == j) frameIcon = IconFonts.FontAwesome5.DotCircle; 

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Text(frameIcon);
            ImGui.PopStyleColor();
            
            var name = (frameSet.Name != string.Empty) ? frameSet.Name : "No name";
            Widget.ImGuiWidget.TextTooltip($"{name} ({j})");
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) 
            {
              var modified = (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left));
              _animEditor.SelectFrame(j, i, modified);
              _modifiedFrame = -1;
              if (modified) _modifiedFrame = j;
            }
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
            {
              _isOpenFrameOptions = true;
              _frameOnOpenOptions = frameSet;
            } 
            Widget.ImGuiWidget.SpanX(10f);
          }

        }
        ImGui.EndTable();
      }
      ImGui.PopStyleVar();
    }
    int _modifiedFrame = -1;
    // Separator with frame numbers 
    void DrawFrameHeader()
    {
      for (int i = 0; i < Animator.Animation.TotalFrames; i++)
      {
        var frame = Animator.Animation.Frames[i];
        ImGui.Text(i.ToString());
        Widget.ImGuiWidget.SpanX(10f);

      }
    }

    string GetPlayString(AnimationPlayer anim)
    {
      if (anim.IsPaused || anim.IsFinished) return IconFonts.FontAwesome5.Play;
      return IconFonts.FontAwesome5.Pause;
    }
  }
}
