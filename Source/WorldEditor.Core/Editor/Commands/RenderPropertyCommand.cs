

namespace Raven
{
  class RenderPropTransformModifyCommand : Command
  {
    internal RenderProperties _props;
    Transform _last;
    Transform _start;

    public RenderPropTransformModifyCommand(RenderProperties props, Transform start)
    {
      _props = props;
      _last = props.Transform.Copy();
      _start = start.Copy(); 
    }

    internal override void Redo()
    {
      _props.Transform = _last.Copy();
    }
    internal override void Undo()
    {
      _props.Transform = _start.Copy();
    }
  }
}
