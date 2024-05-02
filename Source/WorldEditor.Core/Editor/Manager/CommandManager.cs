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
      _sprites = sprites.Copy();
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
  class ModifyTileCommand : Command
  {
    Tile _tile;
    Tile _start;
    Tile _last;

    public ModifyTileCommand(Tile tile, Tile start)
    {
      _tile = tile.Copy();
      _start = start.Copy();
      _last = tile.Copy();
    }
    internal override void Redo()
    {
      _tile = _last.Copy();
    }
    internal override void Undo()
    {
      _tile = _start.Copy();
    }
  }
  class RenamePropertiedCommand : Command
  {
    IPropertied _prop;
    string _item;
    string _start;
    string _last;

    public RenamePropertiedCommand(IPropertied prop, string item, string start)
    {
      _prop = prop;
      _item = item;
      _start = start;
      _last= item;
    }
    internal override void Redo()
    {
      _prop.Properties.Rename(_item, _last);
      _item = _last;
    }
    internal override void Undo()
    {
      _prop.Properties.Rename(_item, _start);
      _item = _start;
    }
  }
  class ModifyPropertiedCommand : Command
  {
    IPropertied _prop;
    KeyValuePair<string, object> _item;
    KeyValuePair<string, object> _start;
    KeyValuePair<string, object> _last;

    public ModifyPropertiedCommand(IPropertied prop, KeyValuePair<string, object> item, KeyValuePair<string, object> start)
    {
      _prop = prop;
      _item = PropertyList.GetCopy(item);
      _start = PropertyList.GetCopy(start);
      _last= PropertyList.GetCopy(item);
    }
    internal override void Redo()
    {
      var item = PropertyList.GetCopy(_last);
      _prop.Properties.Set(item.Key, item.Value);
    }
    internal override void Undo()
    {
      var item = PropertyList.GetCopy(_start);
      _prop.Properties.Set(item.Key, item.Value);
    }
  }
  class AddPropertiedCommand : Command
  {
    IPropertied _prop;
    KeyValuePair<string, object> _item;

    public AddPropertiedCommand(IPropertied prop, KeyValuePair<string, object> item)
    {
      _prop = prop;
      _item = PropertyList.GetCopy(item);
    }
    internal override void Redo()
    {
      var item = PropertyList.GetCopy(_item);
      _prop.Properties.Add(item.Value, item.Key);
    }
    internal override void Undo()
    {
      _prop.Properties.Remove(_item.Key);
    }
  }
  class AddTileCommand : Command
  {
    internal readonly Sheet _sheet;
    Tile _tile;

    public AddTileCommand(Sheet sheet, Tile tile)
    {
      _sheet = sheet;
      _tile = tile;
    }
    internal override void Redo()
    {
      _sheet.CreateTile(_tile);
    }
    internal override void Undo()
    {
      _sheet.TileMap.Remove(_tile.Id);
    }
  }
  class AddSpriteSceneCommand : Command
  {
    internal readonly Sheet _sheet;
    SpriteScene _scene;

    public AddSpriteSceneCommand(Sheet sheet, SpriteScene scene)
    {
      _sheet = sheet;
      _scene = scene;
    }
    internal override void Redo()
    {
      _sheet.AddScene(_scene);
    }
    internal override void Undo()
    {
      _sheet.RemoveScene(_scene);
    }
  }
  class RenameSpriteSceneCommand : Command
  {
    SpriteScene _scene;
    string _last;
    string _start;

    public RenameSpriteSceneCommand(SpriteScene scene, string start)
    {
      _scene = scene;
      _start = start;
      _last = scene.Name;
    }
    internal override void Redo()
    {
      _scene.Name = _last;
    }
    internal override void Undo()
    {
      _scene.Name = _start;
    }
  }
  // There SHOULD be no need to worry about parts that are removed later in the process
  // because, before this command is undone, the removed part will be undone first (as you cannot jump in the command history) and the whole state will 
  // be exactly the same before this command is called
  class ModifyScenePartCommand : Command
  {
    SpriteScene _scene;
    string _name;
    ISceneSprite _last;
    ISceneSprite _start;

    public ModifyScenePartCommand(SpriteScene scene, ISceneSprite sprite, ISceneSprite start)
    {
      _scene = scene;
      _name = sprite.Name;
      _start = start.Copy();
      _last = sprite.Copy();
    }
    internal override void Redo()
    {
      _scene.ReplaceSprite(_name, _last.Copy());
      _name = _last.Name;
    }
    internal override void Undo()
    {
      _scene.ReplaceSprite(_name, _start.Copy());
      _name = _start.Name;
    }
  }
  class AddScenePartCommand : Command
  {
    internal readonly SpriteScene _scene;
    ISceneSprite _sprite;

    public AddScenePartCommand(SpriteScene scene, ISceneSprite sprite)
    {
      _sprite = sprite;
      _scene = scene;
    }
    internal override void Redo()
    {
      _scene.AddSprite(_sprite);
    }
    internal override void Undo()
    {
      _scene.RemoveSprite(_sprite.Name);
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
