using Microsoft.Xna.Framework;

namespace Raven
{
  class AddLevelCommand : Command
  {
    internal readonly World _world;
    Level _level;

    public AddLevelCommand(World world, Level level)
    {
      _world = world;
      _level = level;
    }
    internal override void Redo()
    {
      _world.AddLevel(_level);
    }
    internal override void Undo()
    {
      _world.RemoveLevel(_level);
    }
  }
  class RemoveLevelCommand : ReversedCommand
  {
    public RemoveLevelCommand(World world, Level level) : base(new AddLevelCommand(world, level)) {} 
  }

  class LevelResizeCommand : Command
  {
    internal readonly Level _level;
    Level _start;
    Level _last;
    Level _world { get => _level.World.GetLevel(_level.Name); }

    public LevelResizeCommand(Level level, Level start)
    {
      _level = level;
      _start = start.Copy();
      _last = level.Copy();
    }

    internal override void Redo()
    {
      var last = _last.Copy();
      _level.Layers = last.Layers;
      _level.ContentSize = last.ContentSize;
    }
    internal override void Undo()
    {
      var start = _start.Copy();
      _level.Layers = start.Layers;
      _level.ContentSize = start.ContentSize;
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
}
