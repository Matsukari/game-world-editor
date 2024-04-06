using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven
{
	  public class WorldViewInputHandler : EditorInterface, IInputHandler
  {
    readonly WorldView _view;
    public event Action<LevelEntity, int> OnLeftClickLevel;
    public event Action<LevelEntity, int> OnRightClickLevel;
    public event Action<Vector2> OnRightClickWorld;
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

      var selectLevel = false;
      for (var i = 0; i < _view.WorldEntity.Levels.Count(); i++)
      {
        var level = _view.WorldEntity.Levels[i];
        if (!level.Enabled) continue;

        if (Nez.Input.LeftMouseButtonPressed 
            && level.Bounds.Contains(Camera.MouseToWorldPoint()) 
            && _view.SpritePicker.SelectedSprite == null 
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
      if (Nez.Input.RightMouseButtonPressed && !selectLevel && OnRightClickWorld != null)
      {
        OnRightClickWorld(Camera.MouseToWorldPoint());
        return true;
      }
      return false;
    }
  }
}