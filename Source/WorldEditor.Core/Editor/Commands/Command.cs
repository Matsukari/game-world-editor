

namespace Raven 
{
  
  /// <summary>
  /// Commands are actions in the editor that describes changes on something that can be undone and redone. Commands does not do something; something must be already done and changed and that change is the data this command holds, to which the command can read either to undo or redo.
  /// </summary> 
  public abstract class Command
  {
    public object Context;

    internal Action OnUndo;

    internal Action OnRedo;

    internal abstract void Redo();

    internal abstract void Undo();

    public virtual string GetName() => GetType().Name;
  }
}
