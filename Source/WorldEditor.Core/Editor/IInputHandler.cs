

namespace Raven 
{
  public interface IInputHandler
  {
    /// <summary>
    /// Hihger numbers go first
    /// </summary> 
    public int Priority() => 0;

    public bool CanHandleInput() => true;

    public bool OnHandleInput(InputManager input) => false;

    /// <summary>
    /// Called when first-came or higher ordered IInputHandler returns true and cannot handle any further events
    /// </summary> 
    public void OnInputBlocked(InputManager input) {}

    /// <summary>
    /// Called when ImGUi blocked the input
    /// </summary> 
    public void OnGuiIntercept(InputManager input) {}

  }
}
