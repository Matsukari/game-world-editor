
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
      Vector2 menubarSize = new Vector2();

      if (ImGui.BeginMainMenuBar())
      {
        menubarSize = ImGui.GetWindowSize();
        if (ImGui.BeginMenu("Project"))
        {

        }
        if (ImGui.BeginMenu("Worlds"))
        {

        }
        if (ImGui.BeginMenu("Sheets"))
        {

        }
        if (ImGui.BeginMenu("View"))
        {

        }
        ImGui.EndMainMenuBar();
      }
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking);
      var position = new Vector2(0f, menubarSize.Y-1);
      var size = Screen.Size;
      size.Y = 45;
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());

      ImGui.PushStyleColor(ImGuiCol.Text, Editor.ColorSet.ViewbarSpecialButton.ToImColor());
      ImGui.Dummy(new System.Numerics.Vector2(270f, 0f));
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.MousePointer))
      {

      }
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.ArrowsAlt))
      {

      }
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.HandSpock))
      {

      }
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.Expand))
      {

      }
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.SyncAlt))
      {

      }
      ImGui.PopStyleColor();
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(10, 0));
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.Th))
      {

      }
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.EllipsisV))
      {

      }
      ImGui.SameLine();

      ImGui.Dummy(new System.Numerics.Vector2(20, 0));
      ImGui.SameLine();

      if (ImGui.Button(IconFonts.FontAwesome5.MousePointer))
      {

      }
      ImGui.SameLine();
      if (ImGui.Button(IconFonts.FontAwesome5.Shapes))
      {

      }
      ImGui.SameLine();

      ImGui.Dummy(new System.Numerics.Vector2(20, 0));
      ImGui.SameLine();
      
      ImGui.PushStyleColor(ImGuiCol.Text, Editor.ColorSet.ShapeButtonTextColor.ToImColor());
      // Draw shape annotators
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
      ImGui.PopStyleColor();

      ImGui.End();
    }
  }
}
