using Microsoft.Xna.Framework;

namespace Raven 
{
  class SceneSpriteListTransformModifyCommand : Command
  {
    internal readonly List<ISceneSprite> _sprites;
    List<Transform> _last;
    List<Transform> _start;

    public SceneSpriteListTransformModifyCommand(List<ISceneSprite> sprites, List<Transform> start)
    {

      _sprites = new List<ISceneSprite>();
      foreach (var sprite in sprites)
      {
        _sprites.Add(sprite);
      }
      _last = new List<Transform>();
      foreach (var s in sprites)
      {
        _last.Add(s.Transform.Duplicate());
      }
      _start = start.CloneItems(); 
    }

    internal override void Redo()
    {
      for (int i = 0; i < _sprites.Count(); i++)
      {
        _sprites[i].Transform = _last[i].Duplicate();
      }
    }
    internal override void Undo()
    {
      for (int i = 0; i < _sprites.Count(); i++)
      {
        _sprites[i].Transform = _start[i].Duplicate();
      }
    }

  }
  class TransformModifyCommand : Command
  {
    Transform _transform;
    Transform _last;
    Transform _start;

    public TransformModifyCommand(Transform transform, Transform start)
    {
      _transform = transform;
      _last = _transform;
      _start = start; 
    }

    internal override void Redo()
    {
      _transform = _last.Duplicate();
    }
    internal override void Undo()
    {
      _transform = _start.Duplicate();
    }

  }
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
    internal readonly Level _level;
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
  public class CommandManager : Nez.GlobalManager
  {
    List<Command> _commands = new List<Command>();
    Command _node { get => _commands[_current]; }
    int _current = -1;

    public int Current { get => _current; }
    public List<Command> Commands { get => _commands; }
    public event Action<Command> OnRecord;
    
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
    public void Record(Command command)
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
    }
    public void Record(Command command, Action onUndoRedo) 
    {
      command.OnUndo = onUndoRedo;
      command.OnRedo = onUndoRedo;
      Record(command);
    }
  }
}
