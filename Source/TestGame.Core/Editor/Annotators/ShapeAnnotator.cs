
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven
{
  public class ShapeAnnotator : IInputHandler
  {
    PrimitiveBatch _primitiveBatch;
    IPropertied _propertied;
    bool _annotating;
    object _shape;

    public bool IsAnnotating { get => _annotating; }
   
    public event Action OnAnnotateStart;
    public event Action OnAnnotateEnd;

    public ShapeAnnotator()
    {
      _primitiveBatch = new PrimitiveBatch();
      _annotating = false;
    }

    public static bool IsShape(object shape) =>
         shape is RectangleModel
      || shape is PointModel
      || shape is PolygonModel
      || shape is EllipseModel;

    public void Annotate(IPropertied property, object shape)
    {
      if (!IsShape(shape)) throw new Exception();
      Mouse.SetCursor(MouseCursor.Crosshair);
      _shape = shape;
    }
    Vector2 _initialMouse = Vector2.Zero;

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
        _primitiveBatch.Begin(projection: camera.ProjectionMatrix, view: camera.ViewProjectionMatrix);
        if (_shape is RectangleModel m1) m1.Render(_primitiveBatch, batcher, camera, color);
        else if (_shape is EllipseModel m2) m2.Render(_primitiveBatch, batcher, camera, color);
        else if (_shape is PointModel m3) m3.Render(_primitiveBatch, batcher, camera, color);
        else if (_shape is PolygonModel m4) m4.Render(_primitiveBatch, batcher, camera, color);
        batcher.FlushBatch();
        _primitiveBatch.End();
      }
      // Released; add 
      else if (input.IsDragLast && _initialMouse != Vector2.Zero && !(_shape is PolygonModel)) 
        Add();
    }

    // Draws all shapes in the properties
    public static void DrawPropertiesShapes(IPropertied propertied, PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      foreach (var prop in propertied.Properties)
      {
        if (prop.Value is Shape shape) shape.Render(primitiveBatch, batcher, camera, color);
      }
    }
  }
}
