using ImGuiNET;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetAutoRegionControl : Control 
    {
      int width = 16;
      int height = 16;
      public override void Render()
      {
        if (Editor.EditState != EditingState.AUTO_REGION) return;
        
        ImGui.OpenPopup(Names.AutoRegion);
        ImGui.SetNextWindowFocus();
        if (ImGui.BeginPopupModal(Names.AutoRegion))
        {
          ImGui.InputInt("Tile Width", ref width);
          ImGui.InputInt("Tile Height", ref height);
          if (ImGui.Button("Generate")) Editor.SpriteSheet.Slice(width, height);
          ImGui.SameLine();
          if (ImGui.Button("Done")) 
          {
            ImGui.CloseCurrentPopup();
            Editor.EditState = Editor.PrevEditState;
          }
          ImGui.EndPopup();
        }
      }
    }
  }
}
