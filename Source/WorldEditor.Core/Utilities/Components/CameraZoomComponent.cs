
using Nez;

namespace Raven.Utils.Components
{
  public class CameraZoomComponent : Component, IUpdatable
  {
    public bool ZoomInOnly = true;
    void IUpdatable.Update()
    {
      if (InputManager.IsImGuiBlocking) return;
      if (Nez.Input.MouseWheelDelta != 0 ) 
      {
        float zoomFactor = 1.2f;
        if (Nez.Input.MouseWheelDelta < 0) zoomFactor = 1/zoomFactor;
        var zoom = Entity.Scene.Camera.RawZoom * zoomFactor;
        var delta = (Entity.Scene.Camera.MouseToWorldPoint() - Entity.Scene.Camera.Position) * (zoomFactor - 1);
        Entity.Scene.Camera.Position += delta;
        Entity.Scene.Camera.RawZoom = zoom;
      }
    }
  }
}
