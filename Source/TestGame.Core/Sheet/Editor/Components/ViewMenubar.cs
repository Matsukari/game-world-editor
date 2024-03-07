
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public class ViewMenubar : Editor.SubEntity
  {
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    }    
    void RenderImGui()
    {
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoDecoration);
      var position = Screen.Size/4;
      position.Y = 0f;
      var size = Screen.Size/2;
      size.Y = 15;
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());
      ImGui.SameLine();
      if (ImGui.Button("Rectangle"))
      {
        Editor.GetSubEntity<Annotator>().Annotate(new Shape.Rectangle());
      }
      ImGui.SameLine();
      if (ImGui.Button("Circle")) 
      {
        Editor.GetSubEntity<Annotator>().Annotate(new Shape.Circle());
      }
      ImGui.SameLine();
      if (ImGui.Button("Point"))
      {
        Editor.GetSubEntity<Annotator>().Annotate(new Shape.Point());
      }
      ImGui.SameLine();
      if (ImGui.Button("Polygon"))
      {
        Editor.GetSubEntity<Annotator>().Annotate(new Shape.Polygon());
      }
      ImGui.End();
    }
  }
}
