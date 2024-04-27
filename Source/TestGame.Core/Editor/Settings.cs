using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven
{
  public class Settings : Widget.Window
  {
    readonly EditorSettings _settings;
    public event Action OnSaveSettings;
    public Settings(EditorSettings settings) 
    {
      _settings = settings;
      IsOpen = false;
    }
    public void Theme()
    {
      var colorY = Math.Min(ImGui.GetWindowHeight(), ImGui.GetContentRegionAvail().Y);
      colorY -= 20;

      if (ImGui.CollapsingHeader("Window", ImGuiTreeNodeFlags.DefaultOpen))
      {
        ImGui.BeginChild("window-theme", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, colorY));
        for (int i = 0; i < _settings.ImGuiColors.Count; i++)
        {
          var numerics = _settings.ImGuiColors[i]; 
          var name = Enum.GetName(typeof(ImGuiCol), i).PascalToWords();
          if (ImGui.ColorEdit4(name, ref numerics, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) 
          {
            _settings.ImGuiColors[i] = numerics;
            _settings.ApplyImGui();
          }
        }
        ImGui.EndChild();
      }
      if (ImGui.CollapsingHeader("Miscellaneous", ImGuiTreeNodeFlags.DefaultOpen))
      {
        ImGui.BeginChild("miscellaneous-theme", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, colorY));
        var fields = _settings.Colors.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        ImGui.SeparatorText("Miscellaneous");
        foreach (var field in fields)
        {
          var colorField = field.GetValue(_settings.Colors);
          var name = field.Name.PascalToWords();
          if (colorField is Vector4 vec)
          {
            var numerics = ((Vector4)field.GetValue(_settings.Colors)).ToNumerics();
            if (ImGui.ColorEdit4(name, ref numerics, 
                  ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.HDR)) field.SetValue(_settings.Colors, numerics.ToVector4());
          }
        }
        ImGui.EndChild();
      }

    }
    public void General()
    {
      ImGui.Checkbox("Highlight Current Layer", ref _settings.Graphics.HighlightCurrentLayer);
      ImGui.Checkbox("Draw Grid on Sheets", ref _settings.Graphics.DrawSheetGrid);
      ImGui.Checkbox("Draw Grid on Levels", ref _settings.Graphics.DrawLayerGrid);

      ImGui.NewLine();
      ImGui.Checkbox("Right Click To Remove Brush", ref _settings.RightClickRemoveBrush);
      ImGui.Checkbox("Right Click To Erase", ref _settings.RightClickErase);

    }
    public void Shortcuts()
    {
      var fields = _settings.Hotkeys.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      if (ImGui.BeginTable("##split", 2, ImGuiTableFlags.Resizable))
      {
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableSetupColumn("names");
        ImGui.TableSetupColumn("keys");
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(1);

        foreach (var field in fields)
        {
          var keyField = field.GetValue(_settings.Hotkeys);
          var name = field.Name.PascalToWords();
          if (keyField is KeyCombination keyCombination)
          {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(name);
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(keyCombination.GetKeyString(0));
          }
        }
        ImGui.EndTable();
      }
    }
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (ImGui.BeginTabBar("settings-tab"))
      {
        ImGuiUtils.TabItem("General", General);
        ImGuiUtils.TabItem("Theme", Theme);
        ImGuiUtils.TabItem("Shortcuts", Shortcuts);
        ImGui.EndTabBar();
      }
      var size = new System.Numerics.Vector2(ImGui.GetWindowWidth() * 0.15f, 20);

      ImGui.Dummy(new System.Numerics.Vector2(0f, ImGui.GetContentRegionAvail().Y - size.Y));
      if (ImGui.Button("Cancel", size))
      {
        IsOpen = false;
      }
      ImGui.SameLine();

      if (ImGui.Button("Save", size))
      {
        if (OnSaveSettings != null) OnSaveSettings();
        IsOpen = false;
      }
    }
  }
}


