using Nez;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Raven.Utils.Components
{
  public class CameraMoveComponent : Component, IUpdatable
  {
    Vector2 _initialSheetPosition = new Vector2();
    void IUpdatable.Update()
    {
      var input = Core.GetGlobalManager<Raven.Input.InputManager>();
      if (input.IsDragFirst)
      {
        _initialSheetPosition = Entity.Scene.Camera.Position;
      }
      if (input.IsDrag && input.MouseDragButton == 2) 
      {
        Entity.Scene.Camera.Position = _initialSheetPosition + (input.MouseDragStart - ImGui.GetIO().MousePos) / Entity.Scene.Camera.RawZoom;
      } 
    }
  }
}
