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
