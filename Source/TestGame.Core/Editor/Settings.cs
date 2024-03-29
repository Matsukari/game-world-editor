using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven.Sheet
{
  public class Settings : EditorComponent
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
            if (colorField is Vector4 vec)
            {
              var numerics = ((Vector4)field.GetValue(_colors)).ToNumerics();
              if (ImGui.ColorEdit4(field.Name, ref numerics, 
                    ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) field.SetValue(_colors, numerics.ToVector4());
            }
          }
          ImGui.EndTabItem();
        }
        ImGui.EndTabBar();
      }
      if (ImGui.Button("Cancel"))
      {
        Enabled = false;
      }
      if (ImGui.Button("Save"))
      {
        Editor.Component<Serializer>().SaveSettings();
        Enabled = false;
      }
      ImGui.End();
    }
  }
}


