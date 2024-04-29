
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven
{
  public class ShapeAnnotator : IInputHandler, IImGuiRenderable
  {
    EditorSettings _settings;
    IPropertied _propertied;
    ShapeModel _shape;
    bool _annotating;

    public bool IsAnnotating { get => _annotating; }
   
    public event Action OnAnnotateStart;
    public event Action OnAnnotateEnd;

    public event Action<ShapeModel, IPropertied> PostProcess;

    public ShapeAnnotator(EditorSettings settings)
    {
      _settings = settings;
      _annotating = false;
    }

    public void Annotate(IPropertied property, ShapeModel shape)
    {
      Mouse.SetCursor(MouseCursor.Crosshair);
      _propertied = property;
      _shape = shape.Duplicate() as ShapeModel;
      Insist.IsNotNull(_propertied);
      Insist.IsNotNull(_shape);
    }
    Vector2 _initialMouse = Vector2.Zero;
    bool _isDrag = false;

    int IInputHandler.Priority() => 10;

    bool IInputHandler.OnHandleInput(InputManager input)
    {
      if (_shape == null) return false;
      if (input.IsDragFirst) 
      {
        Console.WriteLine("Started annotating");
        _initialMouse = Vector2.Zero;
        _annotating = true;
        _isDrag = true;

      }
      if (input.IsDragLast && _shape is not PolygonModel) Finish();

      return true;
    }
    // Adds to current context
    void Finish()
    {
      Mouse.SetCursor(MouseCursor.Arrow);
      if (PostProcess != null) PostProcess(_shape, _propertied);
      _propertied.Properties.Add(_shape);
      _initialMouse = Vector2.Zero;
      _annotating = false;
      _shape = null;
      _isDrag = false;
      Console.WriteLine("Finished");
    }
    void IImGuiRenderable.Render(ImGuiWinManager imgui)
    {

      if (!_annotating) return;

      var input = Core.GetGlobalManager<InputManager>();


      // start point
      if (_initialMouse == Vector2.Zero)
      {
        _initialMouse = input.Camera.MouseToWorldPoint();
        if (OnAnnotateStart != null) OnAnnotateStart();
        if (_shape is PointModel shape)
        {
          shape.Bounds = new RectangleF(_initialMouse, Vector2.Zero);
          Finish();
          return;
        }
        else if (_shape is PolygonModel poly)
        {
          poly.Points.Add(_initialMouse);
          if (poly.Points.Count() >= 3 && Collisions.CircleToPoint(poly.Points[0], 10, _initialMouse))
          {
            Finish();
            return;
          }
        }
      }

      // Dragging
      if (_isDrag) 
      {
        _shape.Render(ImGuiNET.ImGui.GetBackgroundDrawList(), input.Camera, _settings.Colors.ShapeActive.ToColor(), _settings.Colors.ShapeOutlineActive.ToColor());
      }

      if (_shape is PolygonModel) return;

      // calculate position of area between mous drag
      var rect = input.MouseDragArea;
      rect.Location = _initialMouse;
      rect.Size = input.Camera.MouseToWorldPoint() - _initialMouse; 
      _shape.Bounds = rect;

    }
  }
}
