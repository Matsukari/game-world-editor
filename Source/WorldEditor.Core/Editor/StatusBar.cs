
using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{
  public class StatusBar : Widget.Window
  {
    readonly Editor _editor;
    public StatusBar(Editor editor)
    {
      _editor = editor;
    }
    public override void Render(ImGuiWinManager imgui)
    {
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking);
      var size = Screen.Size;
      size.Y = 27;
      var position = new Vector2(0f, Screen.Height-size.Y-1);
      ImGui.SetWindowPos(position.ToNumerics());
      ImGui.SetWindowSize(size.ToNumerics());

      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize(); 

      ImGuiUtils.SpanX(20f);

      var zoom = (int)(_editor.Scene.Camera.RawZoom * 100);
      ImGui.SameLine();
      if (ImGui.Button(Icon.SearchMinus)) _editor.Scene.Camera.RawZoom *= 0.5f;
      ImGui.SameLine();
      ImGui.SetNextItemWidth(40);
      if (ImGui.DragInt("##1", ref zoom)) _editor.Scene.Camera.RawZoom = zoom / 100f;
      ImGui.SameLine();
      if (ImGui.Button(Icon.SearchPlus)) _editor.Scene.Camera.RawZoom *= 2f;

      if (_editor.ContentManager.View is SheetView sheetView)
      {
        ImGuiUtils.SpanX(10);
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
