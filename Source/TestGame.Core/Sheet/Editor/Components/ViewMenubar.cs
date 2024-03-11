
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
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus);
      var position = new Vector2(0f, menubarSize.Y);
      var size = Screen.Size;
      size.Y = 40;
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());


      // ImGui.Dummy(new System.Numerics.Vector2(300, 0f));
      // ImGui.SameLine();


      ImGui.Dummy(new System.Numerics.Vector2(300, 0f));
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
