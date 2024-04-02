
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  public class StatusBar : EditorComponent, IImGuiRenderable
  {
    public void Render(Editor editor)
    {
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking);
      var size = Screen.Size;
      size.Y = 27;
      var position = new Vector2(0f, Screen.Height-size.Y-1);
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());


      ImGui.Dummy(new System.Numerics.Vector2(20f, 0f));

      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(10, 0));
      ImGui.SameLine();
      var zoom = (int)(Entity.Scene.Camera.RawZoom * 100);
      ImGui.TextDisabled($"Zoom: {zoom}%%");

      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(10, 0));
      ImGui.SameLine();
      ImGui.TextDisabled($"State: {Editor.EditState}");

      var worldView = Editor.GetEditorComponent<WorldView>();
      var worldEditor = Editor.GetEditorComponent<WorldEditor>();
      if (Editor.GetEditorComponent<SheetView>().Enabled)
      {
        ImGui.SameLine();
        ImGui.Dummy(new System.Numerics.Vector2(10, 0));
        ImGui.SameLine();
        ImGui.Text($"Tile: {Editor.GetEditorComponent<SheetView>().TileInMouse.Location.SimpleStringFormat()}");
      }
      else if (worldView.Enabled)
      {
        if (worldEditor.SelectedSprite != null)
        {
          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(20, 0));
          ImGui.SameLine();
          ImGui.Text($"Paint {worldEditor.PaintMode} > {worldEditor.PaintType}");
        }
      }
    
      ImGui.End();
    }
  }
}
