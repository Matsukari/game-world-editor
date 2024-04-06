using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Raven
{
  public interface IInputHandler
  {
    /// <summary>
    /// Hihger numbers go first
    /// </summary> 
    public int Priority() => 0;

    public bool OnHandleInput(InputManager input) => false;

    /// <summary>
    /// Called when some IInputHandler returns true and cannot handle any further events
    /// </summary> 
    public void OnInputBlocked(InputManager input) {}

  }
  public class InputManager : GlobalManager
  {
    public bool IsDrag = false;
    public bool IsDragFirst = false;
    public bool IsDragLast = false; 
    public bool IsImGuiBlocking { get => ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) || ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow); }

    public int MouseDragButton = -1; 
    public RectangleF MouseDragArea = new RectangleF();
    public Num.Vector2 MouseDragStart = new Num.Vector2();

    public List<IInputHandler> InputHandlers { get => _inputHandlers; }

    List<IInputHandler> _inputHandlers = new List<IInputHandler>();

    public void RegisterInputHandler(IInputHandler handler)
    {
      _inputHandlers.Add(handler);
      _inputHandlers.OrderByDescending(item => item.Priority());
    }
    public override void Update()
    {
      HandleGuiDrags();

      if (IsImGuiBlocking) return;

      for (int i = 0; i < _inputHandlers.Count; i++)
      {
        if (_inputHandlers[i].OnHandleInput(this)) 
        {
          Console.WriteLine($"{_inputHandlers[i].GetType().Name} blocks");
          for (int j = 0; j < _inputHandlers.Count; j++)
          {
            if (i != j) _inputHandlers[j].OnInputBlocked(this);
          }
          break;
        }
      }
    }
    void HandleGuiDrags() 
    {
      var pos = Nez.Input.RawMousePosition;
      if (IsDrag) 
      {
        IsDragFirst = false;
        MouseDragArea.Size = new Vector2(
            pos.X - MouseDragArea.X, 
            pos.Y - MouseDragArea.Y);
        MouseDragArea = MouseDragArea.ConsumePoint(pos.ToVector2().ToNumerics());
      }
      if (!IsDrag && (Nez.Input.LeftMouseButtonPressed || Nez.Input.RightMouseButtonPressed || Nez.Input.MiddleMouseButtonPressed))
      {
        IsDrag = true;
        IsDragFirst = true;
        if (Nez.Input.LeftMouseButtonPressed) MouseDragButton = 0;
        else if (Nez.Input.RightMouseButtonPressed) MouseDragButton = 1;
        else MouseDragButton = 2;
        MouseDragArea.X = pos.X;
        MouseDragArea.Y = pos.Y;
        MouseDragStart.X = pos.X;
        MouseDragStart.Y = pos.Y;
      }
      IsDragLast = false;
      if (IsDrag && (Nez.Input.LeftMouseButtonReleased || Nez.Input.RightMouseButtonReleased || Nez.Input.MiddleMouseButtonReleased))
      {
        IsDrag = false;
        IsDragLast = true;
      }
    }
  }
}
