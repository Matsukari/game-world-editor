using Microsoft.Xna.Framework;

namespace Raven
{
  class RemovePaintTileCommand : Command
  {
    TileLayer _layer;
    internal TileInstance _last;
    Point _coord;

    public RemovePaintTileCommand(TileLayer layer, TileInstance tile, Point coord)
    {
      _layer = layer;
      _last = tile;
      _coord = coord;
    }
    internal override void Redo()
    {
      _layer.RemoveTile(_coord);
    }
    internal override void Undo()
    {
      _layer.ReplaceTile(_coord, _last);
    }
  }
  class PaintTileCommand : Command
  {
    TileLayer _layer;
    internal TileInstance _last;
    internal TileInstance _start;
    Point _coord;

    public PaintTileCommand(TileLayer layer, TileInstance tile, TileInstance start, Point coord)
    {
      _layer = layer;
      _start = start;
      _last = tile;
      _coord = coord;
    }
    internal override void Redo()
    {
      _layer.ReplaceTile(_coord, _last);
    }
    internal override void Undo()
    {
      _layer.ReplaceTile(_coord, _start);
    }
  }
}
