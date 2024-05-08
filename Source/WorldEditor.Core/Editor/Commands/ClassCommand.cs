using System.Reflection;

namespace Raven
{
  class ModifyClassMemberCommand : Command
  {
    internal readonly object _obj;

    string _memberName;
    object _start;
    object _last;
    bool _isProperty;

    public ModifyClassMemberCommand(object obj, string memberName, object last, object start)
    {
      _obj = obj;
      _memberName = memberName;
      try 
      {
        GetField().GetValue(obj);
        _isProperty = false;
      }
      catch (Exception)
      {
        try 
        {
          GetProperty().GetValue(obj);
        }
        catch (Exception)
        {
          Console.WriteLine("No member named: " + memberName);
        }
        _isProperty = true;
      }
      _last = last;
      _start = start;
    }

    public ModifyClassMemberCommand(object obj, string memberName, object start)
    {
      _obj = obj;
      _memberName = memberName;
      try 
      {
        _last = GetField().GetValue(obj);
        _isProperty = false;
      }
      catch (Exception)
      {
        try 
        {
          _last = GetProperty().GetValue(obj);
        }
        catch (Exception)
        {
          Console.WriteLine("No member named: " + memberName);
        }
        _isProperty = true;
      }
      _start = start;
    }

    FieldInfo GetField() => _obj.GetType().GetField(_memberName);
    PropertyInfo GetProperty() => _obj.GetType().GetProperty(_memberName);

    internal override void Redo()
    {
      if (!_isProperty)
        GetField().SetValue(_obj, _last);
      else 
        GetProperty().SetValue(_obj, _last);
    }
    internal override void Undo()
    {
      if (!_isProperty)
        GetField().SetValue(_obj, _start);
      else 
        GetProperty().SetValue(_obj, _last);
    }
  }
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
