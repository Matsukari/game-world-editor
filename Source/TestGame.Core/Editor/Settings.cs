using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;

namespace Raven
{
  public class Settings : EditorComponent
  {
    Editor _editor;
    public Settings() 
    {
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
          var fields = Editor.Settings.Colors.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
          foreach (var field in fields)
          {
            var colorField = field.GetValue(Editor.Settings.Colors);
            if (colorField is Vector4 vec)
            {
              var numerics = ((Vector4)field.GetValue(Editor.Settings.Colors)).ToNumerics();
              if (ImGui.ColorEdit4(field.Name, ref numerics, 
                    ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) field.SetValue(Editor.Settings.Colors, numerics.ToVector4());
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


