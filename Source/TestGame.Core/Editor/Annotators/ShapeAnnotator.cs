
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven.Sheet
{
  public class ShapeAnnotator : EditorComponent
  {
    public ShapeAnnotator() => RenderLayer = -1;
    public void Annotate(Shape shape)
    {
      if (ContentData.ShapeContext == null) throw new MissingFieldException();
      ContentData.ShapeSelection = shape;
      Mouse.SetCursor(MouseCursor.Crosshair);
    }

    Vector2 _initialMouse = Vector2.Zero;
    public override void Render(Batcher batcher, Camera camera)
    {
      var input = Core.GetGlobalManager<Raven.Input.InputManager>();
      if (ContentData.ShapeContext == null || input.IsImGuiBlocking || ContentData.ShapeSelection == null) return;

      // calculate position of area between mous drag
      var rect = input.MouseDragArea;
      rect.Location = _initialMouse;
      rect.Size = camera.MouseToWorldPoint() - _initialMouse;
      ContentData.ShapeSelection.Bounds = rect;

      // Adds to current context
      void Add()
      {
        ContentData.ShapeContext.Properties.Add(ContentData.ShapeSelection);
        ContentData.ShapeSelection = null;
        _initialMouse = Vector2.Zero;
        Mouse.SetCursor(MouseCursor.Arrow);
      }

      // start point
      if (input.IsDragFirst && _initialMouse == Vector2.Zero && Nez.Input.LeftMouseButtonDown)
      {
        Editor.GetEditorComponent<SheetSelector>().RemoveSelection();
        _initialMouse = camera.MouseToWorldPoint();
        if (ContentData.ShapeSelection is Shape.Point)
        {
          rect.Location = _initialMouse;
          rect.Size = new Vector2(20, 30);
          rect.X -= rect.Width/2;
          rect.Y -= rect.Height;
          ContentData.ShapeSelection.Bounds = rect;
          Add();
        }
      }
      // Dragging
      else if (input.IsDrag && Nez.Input.LeftMouseButtonDown) 
      {
        Editor.PrimitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
        ContentData.ShapeSelection.Render(Editor.PrimitiveBatch, batcher, camera, Editor.Settings.Colors.AnnotatedShapeActive);
        Editor.PrimitiveBatch.End(); 
      }
      // Released; add 
      else if (input.IsDragLast && _initialMouse != Vector2.Zero && !(ContentData.ShapeSelection is Shape.Polygon)) Add();
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
