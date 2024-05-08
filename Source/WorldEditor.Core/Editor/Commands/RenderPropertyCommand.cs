

namespace Raven
{
  class RenderPropModifyCommand : ModifyClassMemberCommand
  {
    internal RenderProperties _props;
    Transform _last;
    Transform _start;

    public RenderPropModifyCommand(object obj, string propName, RenderProperties last, RenderProperties start) 
      : base(obj, propName, (last != null) ? last.Copy() : null, (start != null) ? start.Copy(): null) {}
  }
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
