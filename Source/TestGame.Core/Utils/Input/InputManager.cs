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

    public int MouseDragButton = -1; 
    public RectangleF MouseDragArea = new RectangleF();
    public Num.Vector2 MouseDragStart = new Num.Vector2();

    public override void Update()
    {
      HandleGuiDrags();
    }

    void HandleGuiDrags() 
    {
      var pos = ImGui.GetIO().MousePos;
      if (IsDrag) 
      {
        IsDragFirst = false;
        MouseDragArea.Size = new Vector2(
            pos.X - MouseDragArea.X, 
            pos.Y - MouseDragArea.Y);
        MouseDragArea = MouseDragArea.ConsumePoint(pos);
      }
      for (int i = 0; i < 3; i++)
      {
        if (ImGui.GetIO().MouseDown[i] && !IsDrag)
        {
          IsDrag = true;
          IsDragFirst = true;
          MouseDragButton = i;
          MouseDragArea.X = pos.X;
          MouseDragArea.Y = pos.Y;
          MouseDragStart.X = pos.X;
          MouseDragStart.Y = pos.Y;
          break;
        }
      }
      IsDragLast = false;
      if (IsDrag && ImGui.GetIO().MouseReleased[MouseDragButton]) 
      {
        IsDrag = false;
        IsDragLast = true;
      }
    }
  }
}
