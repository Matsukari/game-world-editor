

namespace Raven
{
  class AddSheetCommand : Command
  {
    internal readonly World _world;
    Sheet _sheet;

    public AddSheetCommand(World world, Sheet sheet)
    {
      _world = world;
      _sheet = sheet;
    }
    internal override void Redo()
    {
      _world.AddSheet(_sheet);
    }
    internal override void Undo()
    {
      _world.RemoveSheet(_sheet);
    }
  }
  class RemoveSheetCommand : ReversedCommand
  {
    public RemoveSheetCommand(World world, Sheet sheet) : base(new AddSheetCommand(world, sheet)) {} 
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
  class RemoveTileCommand : ReversedCommand
  {
    public RemoveTileCommand(Sheet sheet, Tile tile) : base(new AddTileCommand(sheet, tile)) {}

  }

}
