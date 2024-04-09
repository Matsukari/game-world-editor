using ImGuiNET;

namespace Raven.Widget
{ 
  public partial class ImGuiWidget
  {
    public static bool ToggleButton(string id, ref bool toggled, uint color)
    { 
      uint backgroundColor = toggled ? color : ImGui.GetColorU32(ImGuiCol.Button);

      ImGui.PushStyleColor(ImGuiCol.Button, backgroundColor);
      var pressed = ImGui.Button(id);
      if (pressed)
        toggled = !toggled;

      ImGui.PopStyleColor();


      return pressed;
    } 
    public static void ToggleButtonGroup(string[] ids, ref bool[] toggles, Action[] actions, Action fallback, uint color, int spacing = 0)
    {
      for (int i = 0; i < ids.Count(); i++)
      {
        ImGui.SameLine();
        if (ToggleButton(ids[i], ref toggles[i], color))
        {
          toggles.FalseRange(i);

          if (!toggles[i] && fallback != null) 
            fallback.Invoke();
          else if (!toggles[i])
            toggles[i] = true;
          else
            actions[i].Invoke();
        }
      }
    }
    public static bool ToggleButton(string id, ref bool toggled) => ToggleButton(id, ref toggled, ImGui.GetColorU32(ImGuiCol.ButtonHovered));

    public static bool DelegateToggleButton(string id, Action callback) => DelegateToogleButton(id, ImGui.GetColorU32(ImGuiCol.ButtonHovered), callback);

    public static void TextTooltip(string descr)
    {
      if (!string.IsNullOrEmpty(descr) && ImGui.IsItemHovered())
      {
        ImGui.BeginTooltip();
        ImGui.Text(descr);
        ImGui.EndTooltip();    
      }
    }
    
    public static bool DelegateToogleButton(string id, uint color, Action callback = null)
    {
      var toggled = false;
      var buttonPressed = ToggleButton(id, ref toggled, color);

      if(buttonPressed)
        callback?.Invoke();

      return buttonPressed;
    }
    public static bool DelegateButton(string id, Action callback = null)
    {
      var buttonPressed = ImGui.Button(id);

      if(buttonPressed)
        callback?.Invoke();

      return buttonPressed;
    }
    public static void ButtonSetFlat(List<(string, Action)> actions, float span = 10)
    {
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(span, 0f)); 
      foreach (var button in actions) 
      {
        ImGui.SameLine();
        if (ImGui.Button(button.Item1)) button.Item2.Invoke();
      }
    }
    public static void ButtonSetFlat(float span, params (string, Action)[] actions)
    {
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(span, 0f)); 
      foreach (var button in actions) 
      {
        ImGui.SameLine();
        if (ImGui.Button(button.Item1)) button.Item2.Invoke();
      }
    }

  }
}
