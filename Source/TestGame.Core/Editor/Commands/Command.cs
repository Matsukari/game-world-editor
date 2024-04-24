

namespace Raven 
{
  
  /// <summary>
  /// Commands are actions in the editor that does something that can be undone and redone. A command does not describe what a change does but what changes it could do to redo and undo the said change. 
  /// </summary> 
  public abstract class Command
  {
    internal Action OnUndo;
    internal Action OnRedo;
    internal abstract void Redo();
    internal abstract void Undo();
  }

  public class CommandGroup : Command
  {
    public List<Command> Commands = new List<Command>();
    internal override void Redo() 
    {
      foreach (var c in Commands) c.Redo();
    }
    internal override void Undo()
    {
      foreach (var c in Commands) c.Undo();
    }
  }
}
