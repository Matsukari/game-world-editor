
using Microsoft.Xna.Framework;
using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
  public class AnimationInspector : Widget.Window
  {
    AnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public Animation Animation { get => Animator.Animation; }
    public bool CanOpen { get => 
             Animator == null 
          || Animation == null 
          || _animEditor.SpriteScene.Animations.Find(item => item.Name == _animEditor.Animation.Name) == null; 
    }

    public AnimationInspector(AnimationEditor animEditor) 
    {
      Name = GetType().Name;
      _animEditor = animEditor;
      NoClose = false;
    }
    public override void Render(ImGuiWinManager imgui)
    {
      // also check if the animation's reference is valid, which may be lost at some point when deleting the current and last animation 
      // while inspcetor is opened
      if (CanOpen) return;
      base.Render(imgui);
      DrawFrameOptions();
    }
    int _cutFrame = -1;
    int _copiedFrame = -1;
    void DrawFrameOptions()
    {
      if (_isOpenFrameOptions)
      {
        _isOpenFrameOptions = false;
        ImGui.OpenPopup("frame-options-popup");
      }
      if (_frameOnOpenOptions != -1 && ImGui.BeginPopup("frame-options-popup"))
      {

        if (_copiedFrame != -1 || _cutFrame != -1)
        {
          int frame;
          if (_cutFrame != -1) frame = _cutFrame;
          else frame = _copiedFrame;
            
          if (ImGui.MenuItem(Icon.Paste + "  Paste"))
          {
            Animation.Insert(Animation.Frames[frame], Animator.CurrentIndex);
          }

          ImGui.Separator();
        }

        if (ImGui.MenuItem(Icon.Trash + "  Delete")) Animation.Frames.RemoveAt(_frameOnOpenOptions);

        if (ImGui.MenuItem(Icon.Cut + "  Cut")) 
        {
          _cutFrame = _frameOnOpenOptions;
          Animation.Frames.RemoveAt(_frameOnOpenOptions);
        }
        if (ImGui.MenuItem(Icon.Copy + "  Copy")) 
        {
          _copiedFrame = _frameOnOpenOptions;
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate")) 
        {
          Animation.Insert(Animation.Frames[_frameOnOpenOptions].Copy(), _frameOnOpenOptions); 
        }
        ImGui.EndPopup();
      }
    }
    Vector2 _trackContentMin = Vector2.Zero;
    public override void OnRender(ImGuiWinManager imgui)
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
      if (ImGui.DragInt("##4", ref currFrame) && currFrame >= 0 && currFrame < Animator.Animation.TotalFrames) Animator.JumpTo(currFrame);

      ImGuiUtils.SpanX(10f);
      Widget.ImGuiWidget.ButtonSetFlat(0f,
        (Icon.Backward,             ()=>Animator.IsReversed = true),
        (Icon.StepBackward,         ()=>Animator.Backward()),
        (GetPlayString(Animator),   ()=>Animator.TooglePlay()),
        (Icon.StepForward,          ()=>Animator.Forward()),
        (Icon.Forward,              ()=>Animator.IsReversed = false)
      );      
      ImGuiUtils.SpanX(10f);
      Widget.ImGuiWidget.DelegateToggleButton(Icon.SyncAlt, ()=>Animator.IsLooping=!Animator.IsLooping);
      ImGuiUtils.SpanX(10f);
      Widget.ImGuiWidget.DelegateButton(Icon.Key, ()=>_animEditor.AddFrameFromCurrentState());

      ImGui.BeginChild("animation-content");
      _trackContentMin = ImGui.GetItemRectMax();
      var childSize = ImGui.GetWindowSize() - _trackContentMin;

      if (Animator.Animation.TotalFrames > 0)
        DrawComponentsTrack();
      else 
      {
        ImGuiUtils.TextMiddle("This animation contains no frames yet. ");
      }

      ImGui.EndChild();
   }
    bool _isOpenFrameOptions = false;
    int _frameOnOpenOptions = -1;
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
        for (int i = 0; i < _animEditor.SpriteScene.Parts.Count(); i++)
        {
          var part = _animEditor.SpriteScene.Parts[i];
          ImGui.TableNextRow();
          ImGui.TableSetColumnIndex(0);
          ImGui.Text(part.Name);
          ImGui.TableSetColumnIndex(1);
          // Draw frames of each components in right side
          for (int j = 0; j < Animator.Animation.TotalFrames; j++)
          {
            var frameSet = Animator.Animation.Frames[j] as SpriteSceneAnimationFrame;
            try 
            {
              var frame = frameSet.Parts[i];
              var color = (Animator.CurrentIndex == j) ? _animEditor.Settings.Colors.FrameActive : _animEditor.Settings.Colors.FrameInactive;
              var frameIcon = Icon.Circle;
              if (Animator.CurrentIndex == j) frameIcon = Icon.DotCircle; 

              ImGui.PushStyleColor(ImGuiCol.Text, color);
              ImGui.Text(frameIcon);
              ImGui.PopStyleColor();

              var name = (frameSet.Name != string.Empty) ? frameSet.Name : "No name";
              Widget.ImGuiWidget.TextTooltip($"{name} ({j})");
              if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) 
              {
                _animEditor.SelectFrame(j, i);
              }
              if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
              {
                _isOpenFrameOptions = true;
                _frameOnOpenOptions = j;
              } 
            }
            catch (Exception)
            {
              ImGui.PushStyleColor(ImGuiCol.Text, _animEditor.Settings.Colors.FrameInactive);
              ImGui.Text(Icon.Minus);
              ImGui.PopStyleColor();
            }
            ImGuiUtils.SpanX(10f);
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
        ImGuiUtils.SpanX(12f);

      }
    }

    string GetPlayString(AnimationPlayer anim)
    {
      if (anim.IsPaused || anim.IsFinished) return Icon.Play;
      return Icon.Pause;
    }
  }
}
