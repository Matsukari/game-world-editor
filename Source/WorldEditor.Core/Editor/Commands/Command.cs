

namespace Raven 
{
  
  /// <summary>
  /// Commands are actions in the editor that describes changes on something that can be undone and redone. Commands does not do something; something must be already done and changed and that change is the data this command holds, to which the command can read either to undo or redo.
  /// </summary> 
  public abstract class Command
  {
    internal Action OnUndo;
    internal Action OnRedo;
    internal abstract void Redo();
    internal abstract void Undo();
  }
  sealed class ReversedCommand : Command
  {
    readonly public Command Command;

    public ReversedCommand(Command command) => Command = command;

    internal override void Undo() => Command.Redo();

    internal override void Redo() => Command.Undo();
  }
  public class CommandGroup : Command
  {
    public List<Command> Commands = new List<Command>();
    public CommandGroup(params Command[] commands)
    {
      Commands = commands.ToList();
    }
    public CommandGroup(List<Command> commands)
    {
      Commands = commands.Copy();
    }
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
