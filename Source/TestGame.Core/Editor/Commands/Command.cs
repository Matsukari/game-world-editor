

namespace Raven 
{
  
  /// <summary>
  /// Commands are actions in the editor that does something that can be undone and redone. A command does not describe what a change does but what changes it could do to redo and undo the said change. 
  /// </summary> 
  public abstract class Command
  {
    internal Action OnUndo;
    internal Action OnRedo;
    internal abstract void Redo();
    internal abstract void Undo();
  }
}
