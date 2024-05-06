

namespace Raven
{
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
}
