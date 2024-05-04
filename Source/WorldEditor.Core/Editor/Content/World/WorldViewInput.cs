using Microsoft.Xna.Framework;
using Nez;

namespace Raven
{
	  public class WorldViewInputHandler : EditorInterface, IInputHandler
  {
    readonly WorldView _view;
    public event Action<Level, int> OnLeftClickLevel;
    public event Action<Level, int> OnRightClickLevel;
    public event Action<Vector2> OnRightClickWorld;
    public event Action<Layer, SpriteSceneInstance> OnHoverScene;
    public event Action<Layer, SpriteSceneInstance> OnLeftClickScene;
    public event Action OnMissScene;
    public readonly TilePainter Painter;

    public WorldViewInputHandler(WorldView view)
    {
      _view = view;
      Painter = new TilePainter(_view);
    }

    bool IInputHandler.OnHandleInput(Raven.InputManager input)
    {
      IInputHandler paintInput = Painter;
      if (paintInput.OnHandleInput(input)) return true;

      var popup = HandleAnnotationsInput(_view.World, input.Camera.MouseToWorldPoint());
      if (popup != null) _view.Window.Popups.ShapePopup = popup;

      var selectLevel = false;
      for (var i = 0; i < _view.World.Levels.Count(); i++)
      {
        // Console.WriteLine("as");
        var level = _view.World.Levels[i];
        if (!level.IsVisible) continue;

        foreach (var shape in level.Properties)
        {
          if (Nez.Input.LeftMouseButtonPressed 
              && shape.Value is ShapeModel model 
              && !_view.CanPaint
              && model.CollidesWith(Camera.MouseToWorldPoint()-level.Bounds.Location))
          {
            Selection.Begin(model.Bounds, model.Icon);
            return true;
          }
        }

        foreach (var layer in level.Layers)
        {
          SpriteSceneInstance scene;
          if (layer is FreeformLayer freeform 
              && freeform.GetSceneAt(Camera.MouseToWorldPoint(), out scene) != -1
              && Nez.Input.LeftMouseButtonReleased 
              && OnLeftClickScene != null)
          {
            OnLeftClickScene(layer, scene);
            return true;
          }
        }

        if (Nez.Input.LeftMouseButtonReleased 
            && level.Bounds.Contains(Camera.MouseToWorldPoint()) 
            && OnLeftClickLevel != null)
        {
          OnLeftClickLevel(level, i);
          return true;
        }
        else if (Nez.Input.RightMouseButtonPressed)
        {
          if (level.Bounds.Contains(Camera.MouseToWorldPoint()) && OnRightClickLevel != null)
          {
            OnRightClickLevel(level, i);
            selectLevel = true;
            return true;
          }
        }
      }
      if (Nez.Input.RightMouseButtonPressed && OnRightClickWorld != null)
      {
        OnRightClickWorld(Camera.MouseToWorldPoint());
        return true;
      }
      return selectLevel;
    }
  }
}
