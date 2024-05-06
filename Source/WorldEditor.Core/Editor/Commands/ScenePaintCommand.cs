using Microsoft.Xna.Framework;

namespace Raven
{
  class RemoveSceneCommand : ReversedCommand
  {
    public RemoveSceneCommand(FreeformLayer layer, SpriteSceneInstance scene) : base(new PaintSceneCommand(layer, scene)) {} 
  }
  class PaintSceneCommand : Command
  {
    FreeformLayer _layer;
    internal SpriteSceneInstance _scene;
    Point _coord;

    public PaintSceneCommand(FreeformLayer layer, SpriteSceneInstance scene)
    {
      _layer = layer;
      _scene = scene;
    }
    internal override void Redo()
    {
      _layer.SpriteScenees.Add(_scene);
    }
    internal override void Undo()
    {
      _layer.SpriteScenees.Remove(_scene);
    }
  }

}
