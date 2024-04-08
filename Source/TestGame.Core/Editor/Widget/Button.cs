using ImGuiNET;
using System.Numerics;

namespace Raven.Widget
{ 
  public partial class ImGuiWidget
  {
    public static Vector2 Rotate(Vector2 v, float cos_a, float sin_a) => new Vector2(v.X * cos_a - v.Y * sin_a, v.X * sin_a + v.Y * cos_a);

    void DrawImage(ImDrawListPtr drawList, nint texture, Vector2 center, Vector2 size, float angle, uint color)
    {
      float cos_a = MathF.Cos(angle);
      float sin_a = MathF.Sin(angle);
      Vector2[] pos = new []
      {
        center + Rotate(new Vector2(-size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
        center + Rotate(new Vector2(+size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
        center + Rotate(new Vector2(+size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a),
        center + Rotate(new Vector2(-size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a)
      };
      Vector2[] uvs = new []
      { 
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(1.0f, 1.0f), 
        new Vector2(0.0f, 1.0f) 
      };

      drawList.AddImageQuad(texture, pos[0], pos[1], pos[2], pos[3], uvs[0], uvs[1], uvs[2], uvs[3], color);
    }
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
    public static void SpanX(float x)
    {
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(x, 0f));
      ImGui.SameLine();
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
