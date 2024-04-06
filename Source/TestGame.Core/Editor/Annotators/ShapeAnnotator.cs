
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven
{
  public class ShapeAnnotator : IInputHandler
  {
    IPropertied _propertied;
    bool _annotating;
    object _shape;

    public bool IsAnnotating { get => _annotating; }
   
    public event Action OnAnnotateStart;
    public event Action OnAnnotateEnd;

    public ShapeAnnotator()
    {
      _annotating = false;
    }

    public void Annotate(IPropertied property, object shape)
    {
      if (!ShapeModelUtils.IsShape(shape)) throw new Exception();
      Mouse.SetCursor(MouseCursor.Crosshair);
      _shape = shape;
    }
    Vector2 _initialMouse = Vector2.Zero;

    int IInputHandler.Priority() => 10;

    bool IInputHandler.OnHandleInput(InputManager input)
    {
      if (input.IsDragFirst && _shape != null) 
      {
        _annotating = true;
      } 
      return _annotating;
    }

    public void Render(Batcher batcher, Camera camera, Color color)
    {
      if (!_annotating) return;

      var input = Core.GetGlobalManager<InputManager>();

      // calculate position of area between mous drag
      var rect = input.MouseDragArea;

      if (_shape is RectangleModel rectangle)
      {
        rect.Location = _initialMouse;
        rect.Size = camera.MouseToWorldPoint() - _initialMouse;
        rectangle.Bounds = rect;
      }
      else if (_shape is EllipseModel ellipse)
      {
        ellipse.Center = _initialMouse;
        ellipse.Width = camera.MouseToWorldPoint().X - _initialMouse.X; 
      }

      // Adds to current context
      void Add()
      {
        Mouse.SetCursor(MouseCursor.Arrow);
        _propertied.Properties.Add(_shape);
        _initialMouse = Vector2.Zero;
        _annotating = false;
        _shape = null;
      }

      // start point
      if (_initialMouse == Vector2.Zero)
      {
        _initialMouse = camera.MouseToWorldPoint();
        if (OnAnnotateStart != null) OnAnnotateStart();
        if (_shape is PointModel shape)
        {
          shape.Position = _initialMouse;
          Add();
        }
      }
      // Dragging
      else if (input.IsDrag && _initialMouse != Vector2.Zero) 
      {
        Editor.PrimitiveBatch.Begin(projection: camera.ProjectionMatrix, view: camera.ViewProjectionMatrix);
        ShapeModelUtils.RenderShapeModel(_shape, Editor.PrimitiveBatch, batcher, camera, color);
        Editor.PrimitiveBatch.End();
      }
      // Released; add 
      else if (input.IsDragLast && _initialMouse != Vector2.Zero && !(_shape is PolygonModel)) 
        Add();
    }
  }
}
