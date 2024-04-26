
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
  public class StatusBar : IImGuiRenderable
  {
    readonly Editor _editor;
    public StatusBar(Editor editor)
    {
      _editor = editor;
    }
    void IImGuiRenderable.Render(ImGuiWinManager imgui)
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
      var zoom = (int)(_editor.Scene.Camera.RawZoom * 100);
      ImGui.TextDisabled($"Zoom: {zoom}%%");

      if (_editor.ContentManager.View is SheetView sheetView)
      {
        ImGui.SameLine();
        ImGui.Dummy(new System.Numerics.Vector2(10, 0));
        ImGui.SameLine();
        ImGui.Text($"Tile: {sheetView.TileInMouse.Location.SimpleStringFormat()}");
      }
      else if (_editor.ContentManager.View is WorldView worldView)
      {
        if (worldView.SpritePicker.SelectedSprite != null)
        {
          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(20, 0));
          ImGui.SameLine();
          ImGui.Text($"Paint {worldView.PaintMode} > {worldView.PaintType}");
        }
      }

      if (!InputManager.IsImGuiBlocking)
      {
        ImGui.GetWindowDrawList().AddRectFilled(
            ImGui.GetWindowPos(), 
            ImGui.GetWindowPos() + new System.Numerics.Vector2(ImGui.GetWindowSize().X, 1), _editor.Settings.Colors.Accent.ToImColor());
      }

      ImGui.End();
    }
  }
}
