

namespace Raven 
{
  public interface IInputHandler
  {
    /// <summary>
    /// Hihger numbers go first
    /// </summary> 
    public int Priority() => 0;

    public bool CanHandleInput() => true;

    public bool CanPassImGui() => false;

    public bool OnHandleInput(InputManager input) => false;

    /// <summary>
    /// Called when some IInputHandler returns true and cannot handle any further events
    /// </summary> 
    public void OnInputBlocked(InputManager input) {}

  }
}
