
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public class ViewStatbar : Editor.SubEntity
  {
    public override void OnAddedToScene()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    }    
    void RenderImGui()
    {
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking);
      var size = Screen.Size;
      size.Y = 45;
      var position = new Vector2(0f, Screen.Height-size.Y-1);
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());

      ImGui.Dummy(new System.Numerics.Vector2(20f, 0f));

      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(10, 0));
      ImGui.SameLine();
      var zoom = (int)(Scene.Camera.RawZoom * 100);
      ImGui.TextDisabled($"Zoom: {zoom}%%");

      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(10, 0));
      ImGui.SameLine();
      ImGui.TextDisabled($"State: {Editor.EditState}");

      if (Editor.GetSubEntity<SheetView>().Enabled)
      {
        ImGui.SameLine();
        ImGui.Dummy(new System.Numerics.Vector2(10, 0));
        ImGui.SameLine();
        ImGui.Text($"Tile: {Editor.GetSubEntity<SheetView>().TileInMouse.Location.SimpleStringFormat()}");
      }

      ImGui.End();
    }
  }
}
