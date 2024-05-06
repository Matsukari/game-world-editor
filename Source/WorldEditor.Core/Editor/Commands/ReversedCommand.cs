

namespace Raven
{
  public class ReversedCommand : Command
  {
    readonly public Command Command;

    public ReversedCommand(Command command) => Command = command;

    internal override void Undo() => Command.Redo();

    internal override void Redo() => Command.Undo();
  }
}
