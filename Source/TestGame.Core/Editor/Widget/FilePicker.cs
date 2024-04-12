using ImGuiNET;

namespace Raven.Widget
{
  public class FilePicker
  {
    bool _isOpenFile = false;
    Action<string> _pickerCallback;
    public string[] AcceptedExtensions;
    public string Title;
    public void Open(Action<string> callback, string title, string[] ext = null)
    {
      if (ext == null)
        ext = Serializer.SheetStdExtensions.Concat(Serializer.WorldStdExtensions).Concat(new []{".png", ".jpg"}).ToArray();

      Title = title;
      _pickerCallback = callback;
      _isOpenFile = true;
      AcceptedExtensions = ext;
    }
    public void Draw(ImGuiWinManager imgui)
    {
      if (_isOpenFile)
      {
        ImGui.OpenPopup("file-picker-modal");
        _isOpenFile = false;
      }
      var isOpen = true;
      if (ImGui.BeginPopupModal("file-picker-modal", ref isOpen, ImGuiWindowFlags.NoTitleBar))
      {
        var picker = FileManWindow.GetFileManWindow(Title, this, Environment.CurrentDirectory, AcceptedExtensions);
        picker.DontAllowTraverselBeyondRootFolder = true;
        if (picker.Draw(imgui))
        {
          _pickerCallback.Invoke(picker.SelectedFile);
          _pickerCallback = null;
          FileManWindow.RemoveFileManWindow(this);
        }
        ImGui.EndPopup();
      }
    }

  }
}
