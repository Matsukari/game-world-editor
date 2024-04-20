

using Microsoft.Xna.Framework;
using ImGuiNET;
using Icon = IconFonts.FontAwesome5;
using Nez;

namespace Raven
{
  public class SpriteAnimationInspector : Widget.Window
  {
    SpriteAnimationEditor _animEditor;
    public AnimationPlayer Animator;
    public Animation Animation { get => Animator.Animation; }
    public bool CanOpen { get => 
        !(  
            Animator == null 
          || Animation == null 
          || (_animEditor.Sheet.Animations.Find(item => item.Name == _animEditor.Animation.Name) == null)); 
    }

    public SpriteAnimationInspector(SpriteAnimationEditor animEditor) 
    {
      Name = GetType().Name;
      _animEditor = animEditor;
      NoClose = false;
    }
    public override void Render(ImGuiWinManager imgui)
    {
      // also check if the animation's reference is valid, which may be lost at some point when deleting the current and last animation 
      // while inspcetor is opened
      if (!CanOpen) return;
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
        ImGui.OpenPopup("sprite-frame-options-popup");
      }
      if (_frameOnOpenOptions != -1 && ImGui.BeginPopup("sprite-frame-options-popup"))
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
    public bool IsGridView = true;
    public override void OnRender(ImGuiWinManager imgui)
    {
      ImGui.Text("Animation: ");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(100);
      ImGui.InputText("##2", ref Animation.Name, 20);

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
      Widget.ImGuiWidget.DelegateToggleButton(Icon.ThLarge, ()=>IsGridView = !IsGridView);

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

      ImGui.NewLine();

      Vector2 thumbnailSize = new Vector2(30, 30);
      int cols = (int)(ImGui.GetContentRegionAvail().X / thumbnailSize.X);

      // Draw components names in left side
      for (int i = 0; i < Animation.Frames.Count(); i++)
      {
        try 
        {
          var frame = Animation.Frames[i] as SpriteAnimationFrame;
          var color = (Animator.CurrentIndex == i) ? _animEditor.Settings.Colors.FrameActive : _animEditor.Settings.Colors.FrameInactive;
          var frameIcon = Icon.Circle;
          if (Animator.CurrentIndex == i) frameIcon = Icon.DotCircle; 

          if (IsGridView)
          {
            ImGuiUtils.DrawImage(frame.Sprite, thumbnailSize.ToNumerics());
            Console.WriteLine("Position " + ImGui.GetItemRectMin());
            if (Animator.CurrentIndex == i)
              ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), color);
          }
          else 
          {
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Text(frameIcon);
            ImGui.PopStyleColor();
          }
          var up = ImGui.GetItemRectMin();
          up.Y -= ImGui.CalcTextSize(i.ToString()).Y;
          ImGui.GetWindowDrawList().AddText(up, color, i.ToString());

          var down = up;
          down.Y += ImGui.GetWindowSize().Y;

          if (Animator.CurrentIndex == i)
            ImGui.GetWindowDrawList().AddLine(up, down, color);

          var name = (frame.Name != string.Empty) ? frame.Name : "No name";
          Widget.ImGuiWidget.TextTooltip($"{name} ({i})");
          if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) 
          {
            _animEditor.SelectFrame(i);
          }
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
          {
            _isOpenFrameOptions = true;
            _frameOnOpenOptions = i;
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
        ImGuiUtils.SpanX(15.5f);

      }
    }

    string GetPlayString(AnimationPlayer anim)
    {
      if (anim.IsPaused || anim.IsFinished) return Icon.Play;
      return Icon.Pause;
    }
  }
}
