using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public class Settings : Component
  {
    Editor _editor;
    GuiColors _colors;
    public Settings(GuiColors colors) 
    {
      // _editor = editor;
      _colors = colors;
    }
    public override void OnAddedToEntity()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
      Enabled = false;
    }
    bool _isOpenColor = false;
    System.Reflection.FieldInfo _fieldOnOpenColor;
    void RenderImGui()
    {
      if (!Enabled) return;  
      ImGui.Begin("Settings");

      if (ImGui.BeginTabBar("settings-tab"))
      {
        if (ImGui.BeginTabItem("Theme"))
        {
          var fields = _colors.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
          foreach (var field in fields)
          {
            var colorField = field.GetValue(_colors);
            if (colorField is Color color)
            {
              var colorText = String.Concat(Enumerable.Repeat(IconFonts.FontAwesome5.SquareFull, 3));
              ImGui.Text($"{field.Name.PadRight(20)}");
              ImGui.SameLine();
              ImGui.PushStyleColor(ImGuiCol.Text, color.ToImColor());
              ImGui.Text(colorText);
              ImGui.PopStyleColor();
              if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
              {
                _fieldOnOpenColor = field;
                _isOpenColor = true;
              }
            }
          }
          ImGui.EndTabItem();
        }
        ImGui.EndTabBar();
      }
      if (ImGui.Button("Close"))
      {
        Enabled = false;
      }
      ImGui.End();

      if (_isOpenColor)
      {
        ImGui.OpenPopup("color-picker");
        _isOpenColor = false;
      }
      if (ImGui.BeginPopupContextItem("color-picker"))
      {
        var numerics = ((Color)_fieldOnOpenColor.GetValue(_colors)).ToVector4().ToNumerics();
        if (ImGui.ColorPicker4(_fieldOnOpenColor.Name, ref numerics, 
              ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) _fieldOnOpenColor.SetValue(_colors, new Color(numerics));
        ImGui.EndPopup();

      }
    }
  }
}


