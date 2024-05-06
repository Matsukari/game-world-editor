

namespace Raven
{
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
  class RemovePropertiedCommand : ReversedCommand
  {
    public RemovePropertiedCommand(IPropertied prop, KeyValuePair<string, object> item) : base(new AddPropertiedCommand(prop, item)) {}
  }
}
