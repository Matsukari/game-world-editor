
namespace Raven 
{ 
  public class CommandManagerHead : Nez.GlobalManager
  {
    public CommandManager Current;
  }

  public class CommandManager
  {
    List<Command> _commands = new List<Command>();
    Command _node { get => _commands[_current]; }
    int _current = -1;

    public int Current { get => _current; }
    public List<Command> Commands { get => _commands; }
    public event Action<Command> OnRecord;

    public bool CanUndo() => _current >= 0;

    public bool CanRedo() => _commands.Count() > _current + 1;
   
    public void Undo()
    {
      if (_current >= 0)
      {
        _node.Undo();
        if (_node.OnUndo != null) _node.OnUndo();
        Console.WriteLine($"Undone {_node.GetType().Name}, current: {_current}, stack: {_commands.Count}");
        _current--;
      }
      else 
        Console.WriteLine($"Nothing to undo");
    }
    public void Redo()
    {
      if (_commands.Count() > ++_current)
      {
        _node.Redo();
        if (_node.OnRedo != null) _node.OnRedo();
        Console.WriteLine($"Redone {_node.GetType().Name}, current: {_current}, stack: {_commands.Count}");
      }
      else 
      {
        _current--;
        Console.WriteLine($"Max stack");
      }
    }
    /// <summary>
    /// Upon modifying, adding, removing, or any changes done in the editor, create a command and record it.
    /// </summary> 
    public Command Record(Command command)
    {
      var next = _current + 1;
      if (next < _commands.Count())
      {
        Console.WriteLine($"Removing {next} - {_commands.Count() - next}");
        _commands.RemoveRange(next, _commands.Count() - next);
      }
      _commands.Add(command);
      _current++;
      Console.WriteLine($"Recording {command.GetType().Name}, stack: {_commands.Count}");

      if (OnRecord != null) OnRecord(command);
      
      return command;
    }
    public Command Record(Command command, Action onUndoRedo) 
    {
      command.OnUndo = onUndoRedo;
      command.OnRedo = onUndoRedo;
      Record(command);
      return command;
    }
    public Command MergeCurrent(Command command) 
    {
      if (_current >= 0) 
      {
        var last = _commands[_current];
        _commands.RemoveAt(_current);
        _commands.Insert(_current, new CommandGroup(last, command));
        Console.WriteLine($"Merged {last} and {command}");
      }
      else
        Record(command);

      return command;
    }
  }
}
