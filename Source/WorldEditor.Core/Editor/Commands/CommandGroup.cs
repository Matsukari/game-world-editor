

namespace Raven
{
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
