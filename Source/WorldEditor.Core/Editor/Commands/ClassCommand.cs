using System.Reflection;

namespace Raven
{
  class ModifyClassFieldCommand : Command
  {
    internal readonly object _obj;

    string _fieldName;
    object _start;
    object _last;

    public ModifyClassFieldCommand(object obj, string fieldName, object start)
    {
      _obj = obj;
      _fieldName = fieldName;
      _last = GetField().GetValue(obj);
      _start = start;
    }

    FieldInfo GetField() => _obj.GetType().GetField(_fieldName);

    internal override void Redo()
    {
      GetField().SetValue(_obj, _last);
    }
    internal override void Undo()
    {
      GetField().SetValue(_obj, _start);
    }
  }
}
