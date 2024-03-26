using ImGuiNET;

namespace Raven.Widget
{
  public class FilePicker
  {
    bool _isOpenFile = false;
    Action<string> _pickerCallback;
    public void Open(Action<string> callback)
    {
      _pickerCallback = callback;
      _isOpenFile = true;
    }
    public void Draw()
    {
      if (_isOpenFile)
      {
        ImGui.OpenPopup("file-picker-modal");
        _isOpenFile = false;
      }
      var isOpen = true;
      if (ImGui.BeginPopupModal("file-picker-modal", ref isOpen, ImGuiWindowFlags.NoTitleBar))
      {
        var picker = Nez.ImGuiTools.FilePicker.GetFilePicker(this, Path.Combine(Environment.CurrentDirectory, "Content"), ".png|.atlas");
        picker.DontAllowTraverselBeyondRootFolder = true;
        if (picker.Draw())
        {
          _pickerCallback.Invoke(picker.SelectedFile);
          _pickerCallback = null;
          Nez.ImGuiTools.FilePicker.RemoveFilePicker(this);
        }
        ImGui.EndPopup();
      }
    }

  }
}
