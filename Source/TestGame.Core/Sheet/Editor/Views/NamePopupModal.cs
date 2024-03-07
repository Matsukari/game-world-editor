using ImGuiNET;


namespace Raven.Sheet
{ 
  public partial class ImGuiViews
  {
    public static string InputName = "";
    public static void NamePopupModal(Editor Editor, string id, Action callback, string label = "Name", uint bufferSize = 10)
    {
      var open = true;
      if (ImGui.BeginPopupModal(id, ref open, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize))
      {
        Editor.Set(Editor.EditingState.Modal);
        ImGui.SetKeyboardFocusHere();
        if (ImGui.InputText(label, ref InputName, bufferSize, ImGuiInputTextFlags.EnterReturnsTrue))
        {
          callback.Invoke();
          Editor.Set(Editor.EditingState.Default);
          ImGui.CloseCurrentPopup();
          InputName = "";
        }
        ImGui.EndPopup();
      }
    }
      
  }
}
