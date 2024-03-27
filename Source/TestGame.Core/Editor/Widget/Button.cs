using ImGuiNET;

namespace Raven.Widget
{ 
  public partial class ImGuiWidget
  {
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
  }
}
