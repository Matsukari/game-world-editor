using ImGuiNET;
using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Raven.Input
{
  public class InputManager : GlobalManager
  {
    public bool IsDrag = false;
    public bool IsDragFirst = false;
    public bool IsDragLast = false; 
    public bool IsImGuiBlocking { get => ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow); }

    public int MouseDragButton = -1; 
    public RectangleF MouseDragArea = new RectangleF();
    public Num.Vector2 MouseDragStart = new Num.Vector2();

    public override void Update()
    {
      HandleGuiDrags();
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
      // if (!IsDrag && (ImGui.GetIO().MouseDown[0] || ImGui.GetIO().MouseDown[1] || ImGui.GetIO().MouseDown[3]))
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
        // Console.WriteLine("first");
      }
      IsDragLast = false;
      if (IsDrag && (Nez.Input.LeftMouseButtonReleased || Nez.Input.RightMouseButtonReleased || Nez.Input.MiddleMouseButtonReleased))
      // if (IsDrag && (ImGui.GetIO().MouseReleased[0] || ImGui.GetIO().MouseReleased[1] || ImGui.GetIO().MouseReleased[3]))
      {
        IsDrag = false;
        IsDragLast = true;
        // Console.WriteLine("last");
      }
    }
  }
}
