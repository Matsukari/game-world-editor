using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public class Settings : Widget.Window
  {
    readonly EditorSettings _settings;
    public Settings(EditorSettings settings) 
    {
      _settings = settings;
    }
    public override void OnRender(Editor editor)
    {
      if (ImGui.BeginTabBar("settings-tab"))
      {
        if (ImGui.BeginTabItem("Theme"))
        {
          var fields = _settings.Colors.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
          foreach (var field in fields)
          {
            var colorField = field.GetValue(_settings.Colors);
            if (colorField is Vector4 vec)
            {
              var numerics = ((Vector4)field.GetValue(_settings.Colors)).ToNumerics();
              if (ImGui.ColorEdit4(field.Name, ref numerics, 
                    ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) field.SetValue(_settings.Colors, numerics.ToVector4());
            }
          }
          ImGui.EndTabItem();
        }
        ImGui.EndTabBar();
      }
      if (ImGui.Button("Cancel"))
      {
        IsOpen = false;
      }
      if (ImGui.Button("Save"))
      {
        editor.Serializer.SaveSettings();
        IsOpen = false;
      }
    }
  }
}


