using Microsoft.Xna.Framework;

namespace Raven 
{
  class TransformMoveCommand : Command
  {
    Transform _transform;
    Vector2 _position;
    Vector2 _lastPosition;

    public TransformMoveCommand(Transform transform)
    {
      _transform = transform;
      _lastPosition = transform.Position;
    }

    internal override void Redo()
    {
      _transform.Position = _position;
    }
    internal override void Undo()
    {
      _transform.Position = _lastPosition;
    }

  }
  class LevelMoveCommand : Command
  {
    Level _level;
    Vector2 _start;
    Vector2 _last;

    public LevelMoveCommand(Level level, Vector2 start)
    {
      _level = level;
      _start = start;
      _last = level.LocalOffset;
    }

    internal override void Redo()
    {
      _level.LocalOffset = _last;
    }
    internal override void Undo()
    {
      _level.LocalOffset = _start;
    }

  }

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
  public class CommandManager : Nez.GlobalManager
  {
    List<Command> _commands = new List<Command>();
    Command _node { get => _commands[_current]; }
    int _current = -1;

    public void Undo()
    {
      try 
      {
        _node.Undo();
        if (_node.OnUndo != null) _node.OnUndo();
        _current--;
        Console.WriteLine($"Undone {_node.GetType().Name}, current: {_current}, stack: {_commands.Count}");
      }
      catch (Exception)
      {
        Console.WriteLine($"Nothing to undo");
      }
    }
    public void Redo()
    {
      try 
      {
        _current++;
        _node.Redo();
        if (_node.OnRedo != null) _node.OnRedo();
        Console.WriteLine($"Redone {_node.GetType().Name}, current: {_current}, stack: {_commands.Count}");
      }
      catch (Exception)
      {
        _current--;
        Console.WriteLine($"Max stack");
      }
    }
    /// <summary>
    /// Upon modifying, adding, removing, or any changes done in the editor, create a command and record it.
    /// </summary> 
    public void Record(Command command)
    {
      if (_current >= 0)
      {
        _commands.RemoveRange(_current, _commands.Count() - 1 - _current);
      }
      _commands.Add(command);
      _current++;
      Console.WriteLine($"Recording {command.GetType().Name}, stack: {_commands.Count}");
    }
    public void Record(Command command, Action onUndoRedo) 
    {
      command.OnUndo = onUndoRedo;
      command.OnRedo = onUndoRedo;
      Record(command);
    }
  }
}
