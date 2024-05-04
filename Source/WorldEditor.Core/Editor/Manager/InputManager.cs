using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Raven
{
  
  public class InputManager : GlobalManager
  {
    public bool IsDrag = false;
    public bool IsDragFirst = false;
    public bool IsDragLast = false; 
    public static Point MouseSnapSize = Point.Zero;
    public static Vector2 ScreenMousePosition 
    {
      get 
      {
        var pos = Nez.Input.MousePosition;
        if (MouseSnapSize != Point.Zero)
        {
          pos = pos.RoundToPoint().ToVector2().RoundFloor(MouseSnapSize);
        }
        return pos;
      }
    }
    public static Vector2 GetWorldMousePosition(Camera camera)
    {
      var pos = camera.MouseToWorldPoint();
      if (MouseSnapSize != Point.Zero)
      {
        pos = pos.RoundToPoint().ToVector2().RoundFloor(MouseSnapSize);
      }
      return pos;
    }
    public static bool IsImGuiBlocking 
    { get => ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) || ImGui.GetIO().WantTextInput || ImGui.GetIO().WantCaptureMouse; }

    public int MouseDragButton = -1; 
    public RectangleF MouseDragArea = new RectangleF();
    public Num.Vector2 MouseDragStart = new Num.Vector2();
    public Num.Vector2 MouseDragStartInWorld = new Num.Vector2();
    public Camera Camera { get => Core.Scene.Camera; }

    public List<IInputHandler> InputHandlers { get => _inputHandlers; }

    public IInputHandler ContentInput;

    List<IInputHandler> _inputHandlers = new List<IInputHandler>();

    public void RegisterInputHandler(IInputHandler handler)
    {
      _inputHandlers.Add(handler);
      _inputHandlers = _inputHandlers.OrderByDescending(item => item.Priority()).ToList();
    }

    public override void Update()
    {
      HandleGuiDrags();

      if (Core.Scene is not EditorScene) return;

      if (IsImGuiBlocking)
      {
        for (int i = 0; i < _inputHandlers.Count; i++)
        {
          _inputHandlers[i].OnGuiIntercept(this);
        }
        return;
      }

      for (int i = 0; i < _inputHandlers.Count; i++)
      {
        if (_inputHandlers[i].CanHandleInput() && _inputHandlers[i].OnHandleInput(this)) 
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
      }
      if (Nez.Input.LeftMouseButtonPressed || Nez.Input.RightMouseButtonPressed || Nez.Input.MiddleMouseButtonPressed)
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
      if (Nez.Input.LeftMouseButtonReleased || Nez.Input.RightMouseButtonReleased || Nez.Input.MiddleMouseButtonReleased)
      {
        IsDrag = false;
        IsDragLast = true;
      }
    }
  }
}
