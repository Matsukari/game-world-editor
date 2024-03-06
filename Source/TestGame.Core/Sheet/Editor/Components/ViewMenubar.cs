
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public class ViewMenubar : Editor.SubEntity
  {
    public void RenderImGui()
    {
      var newAtlas = false;
      var openFile = false;

      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);
      ImGui.SetWindowPos((Screen.Center/4).ToNumerics());
      ImGui.SetWindowSize((Screen.Size/2).ToNumerics());
      if (ImGui.BeginMenuBar())
      {
        if (ImGui.BeginMenu("File"))
        {
          if (ImGui.MenuItem("New Atlas from Folder")) newAtlas = true;
          if (ImGui.MenuItem("Load Atlas or PNG")) openFile = true;
          ImGui.EndMenu();
        }
        if (ImGui.Button("Rectangle"))
        {
          Editor.GetSubEntity<Annotator>().Annotate(new Shape.Rectangle());
        }
        else if (ImGui.Button("Circle")) 
        {
          Editor.GetSubEntity<Annotator>().Annotate(new Shape.Circle());
        }
        else if (ImGui.Button("Point"))
        {
          Editor.GetSubEntity<Annotator>().Annotate(new Shape.Point());
        }

        ImGui.EndMenuBar();
      }
      if (newAtlas) ImGui.OpenPopup("new-atlas");
      if (openFile) ImGui.OpenPopup("open-file");
      Gui.LoadTextureFromFilePopup();
    }
  }
}
