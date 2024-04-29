using System.Numerics;
using ImGuiNET;

namespace Raven
{
  public class EditorSettings
  {
    public EditorColors Colors = new EditorColors();

    public EditorGraphics Graphics = new EditorGraphics();

    [Nez.Persistence.JsonExclude]
    public EditorHotkeys Hotkeys = new EditorHotkeys();

    public List<EditorContentData> LastFiles = new List<EditorContentData>();

    public List<Vector4> ImGuiColors = new List<Vector4>();

    public int LastFile = 0;

    public bool IsEditorBusy = false;

    public bool RightClickRemoveBrush = true;

    public bool RightClickErase = false;

    public bool ShowPopupOnLevelCreate = true;

    public void ApplyImGui()
    {
      for (int i = 0; i < ImGuiColors.Count; i++)
        ImGui.GetStyle().Colors[i] = ImGuiColors[i];
    }
  } 
}
