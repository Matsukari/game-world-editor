using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public class Settings : Component
  {
    Editor _editor;
    EditorColors _colors;
    EditorSettings _settings;
    public Settings(EditorSettings settings) 
    {
      // _editor = editor;
      _settings = settings;
      _colors = settings.Colors;
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
              var numerics = ((Color)field.GetValue(_colors)).ToVector4().ToNumerics();
              if (ImGui.ColorEdit4(field.Name, ref numerics, 
                    ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) field.SetValue(_colors, new Color(numerics));
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
    }
  }
}


