
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven
{
  public class ShapeAnnotator : RenderableComponent, IInputHandler
  {
    EditorSettings _settings;
    IPropertied _propertied;
    ShapeModel _shape;
    bool _annotating;

    public bool IsAnnotating { get => _annotating; }
   
    public event Action OnAnnotateStart;
    public event Action OnAnnotateEnd;

    public ShapeAnnotator(EditorSettings settings)
    {
      _settings = settings;
      _annotating = false;
    }

    public override bool IsVisibleFromCamera(Camera camera) => true;
        
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
      if (input.IsDragLast) Finish();

      return true;
    }
    // Adds to current context
    void Finish()
    {
      Mouse.SetCursor(MouseCursor.Arrow);
      _propertied.Properties.Add(_shape);
      _initialMouse = Vector2.Zero;
      _annotating = false;
      _shape = null;
      _isDrag = false;
      Console.WriteLine("Finished");
    }
    public override void Render(Batcher batcher, Camera camera)
    {

      if (!_annotating) return;

      var input = Core.GetGlobalManager<InputManager>();


      // start point
      if (_initialMouse == Vector2.Zero)
      {
        _initialMouse = camera.MouseToWorldPoint();
        if (OnAnnotateStart != null) OnAnnotateStart();
        if (_shape is PointModel shape)
        {
          shape.Bounds = new RectangleF(_initialMouse, Vector2.Zero);
          Finish();
        }
      }
      // Dragging
      if (_isDrag) 
      {
        Editor.PrimitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
        _shape.Render(Editor.PrimitiveBatch, batcher, camera, (Entity as Editor).Settings.Colors.ShapeActive.ToColor());
        Editor.PrimitiveBatch.End();
      }

      // calculate position of area between mous drag
      var rect = input.MouseDragArea;
      rect.Location = _initialMouse;
      rect.Size = camera.MouseToWorldPoint() - _initialMouse; 
      _shape.Bounds = rect;

    }
  }
}
