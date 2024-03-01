
using ImGuiNET;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetMenubarControl : Control
    {
      public override void Render()
      {
        var newAtlas = false;
        var openFile = false;
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
          }
          if (ImGui.Button("Circle")) 
          {

          }
          if (ImGui.Button("Point"))
          {

          }
          if (ImGui.Button("Polygon"))
          {

          }

          ImGui.EndMenuBar();
        }
        if (newAtlas) ImGui.OpenPopup("new-atlas");
        if (openFile) ImGui.OpenPopup("open-file");
        Gui.LoadTextureFromFilePopup();
      }
    }
  }
}
