using ImGuiNET;

namespace Raven.Widget
{
  public partial class ImGuiWidget
  {
    public static void BreadCrumb(params string[] labels)
    {
      ImGui.PushStyleColor(ImGuiCol.Text, Raven.Sheet.EditorColors.Get(ImGuiCol.TextDisabled));
      for (int i = 0; i < labels.Count()-1; i++)
      {
        ImGui.Text(labels[i]);
        ImGui.SameLine();
      }
      ImGui.PopStyleColor();
      ImGui.Text(labels.Last());
    }
  }
}
