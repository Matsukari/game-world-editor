
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;
using System.Reflection;

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
        var shapeInstance = System.Convert.ChangeType(System.Activator.CreateInstance(shapeType), shapeType) as Shape;
        var icon = shapeType.GetField("Icon", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).GetValue(shapeInstance);
        string label;
        if (icon == null) label = shapeType.Name;
        else label = icon as string;

        if (ImGui.Button(label)) 
          Editor.GetSubEntity<Annotator>().Annotate(shapeInstance);
        ImGui.SameLine();
      }
      ImGui.End();
    }
  }
}
