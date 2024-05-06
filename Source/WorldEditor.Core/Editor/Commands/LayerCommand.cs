

namespace Raven
{
  class AddLayerCommand : Command
  {
    internal readonly Level _level;
    Layer _layer;

    public AddLayerCommand(Level level, Layer layer)
    {
      _level = level;
      _layer = layer;
    }
    internal override void Redo()
    {
      _level.AddLayer(_layer);
    }
    internal override void Undo()
    {
      _level.RemoveLayer(_layer);
    }
  }
  class RemoveLayerCommand : ReversedCommand
  {
    public RemoveLayerCommand(Level level, Layer layer) : base(new AddLayerCommand(level, layer)) {} 
  }
}
