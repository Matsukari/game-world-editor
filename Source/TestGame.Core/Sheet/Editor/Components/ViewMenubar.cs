
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;

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
      var position = new Vector2();
      var size = Screen.Size;
      size.Y = 35;
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());

      foreach (var shapeType in typeof(Shape).GetNestedTypes())
      {
        if (ImGui.Button(shapeType.Name)) 
          Editor.GetSubEntity<Annotator>().Annotate(System.Convert.ChangeType(System.Activator.CreateInstance(shapeType), shapeType) as Shape);
        ImGui.SameLine();
      }
      ImGui.End();
    }
  }
}
