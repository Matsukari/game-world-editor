// using ImGuiNET;
//
// namespace Raven.Sheet
// {
//   public partial class SheetAutoRegionControl : Editor.Window
//   {
//     int _width = 16;
//     int _height = 16;
//     public override void RenderImGui()
//     {
//       if (Editor.EditState != Editor.EditingState.AutoRegion) return;
//
//       ImGui.OpenPopup(Names.AutoRegion);
//       ImGui.SetNextWindowFocus();
//       if (ImGui.BeginPopupModal(Names.AutoRegion))
//       {
//         ImGui.InputInt("Tile Width", ref _width);
//         ImGui.InputInt("Tile Height", ref _height);
//         if (ImGui.Button("Generate")) Editor.SpriteSheet.SetTileSize(_width, _height);
//         ImGui.SameLine();
//         if (ImGui.Button("Done")) 
//         {
//           ImGui.CloseCurrentPopup();
//           Editor.EditState = Editor.PrevEditState;
//         }
//         ImGui.EndPopup();
//       }
//     }
//   }
// }
