using ImGuiNET;


namespace Raven
{
  public class History : Widget.Window
  {
    CommandManager _commandManager;

    public History()
    {
      NoClose = false;
    }
    public void Update(CommandManager man)
    {
      _commandManager = man;
    }
    public override void OnRender(ImGuiWinManager imgui)
    {
      if (_commandManager == null || _commandManager.Commands.Count() == 0)
        ImGuiUtils.TextMiddleX("No history.");

      if (_commandManager == null) return;

      ImGui.BeginChild("command-list");
      for (int i = 0; i < _commandManager.Commands.Count(); i++)
      {
        var open = _commandManager.Current == i;
        ImGui.Selectable($"{i}. {_commandManager.Commands[i].GetType().Name.BestWrap()}", ref open);
      }
      ImGui.EndChild();
    }
  }
}
