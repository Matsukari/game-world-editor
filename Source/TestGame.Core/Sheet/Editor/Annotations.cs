
using Nez;
using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public class Annotator : Editor.SubEntity
  {
    public void Annotate(Shape shape)
    {
      if (Gui.ShapeContext == null) throw new MissingFieldException();
      Editor.Set(Editor.EditingState.AnnotateShape);
      Gui.ShapeSelection = shape;
    }
    public override void OnAddedToScene()
    {
      var renderable = AddComponent(new Renderable());
      renderable.RenderLayer = -1;
    }
    public class Renderable : Editor.SubEntity.RenderableComponent<Annotator>
    {
      Vector2 _initialMouse = Vector2.Zero;
      public override void Render(Batcher batcher, Camera camera)
      {
        var input = Core.GetGlobalManager<Raven.Input.InputManager>();
        if (Editor.EditState != Editor.EditingState.AnnotateShape || input.IsImGuiBlocking) return;

        // calculate position of area between mous drag
        var rect = input.MouseDragArea;
        rect.Location = _initialMouse;
        rect.Size = camera.MouseToWorldPoint() - _initialMouse;
        Gui.ShapeSelection.Bounds = rect;
      
        // Adds to current context
        void Add()
        {
          Gui.ShapeContext.Properties.Add(Gui.ShapeSelection);
          Editor.Set(Editor.EditingState.Default);
          Gui.ShapeSelection = null;
          _initialMouse = Vector2.Zero;
        }

        // start point
        if (input.IsDragFirst && _initialMouse == Vector2.Zero)
        {
          Editor.GetSubEntity<SheetSelector>().RemoveSelection();
          _initialMouse = camera.MouseToWorldPoint();
          if (Gui.ShapeSelection is Shape.Point)
          {
            rect.Location = _initialMouse;
            rect.Size = new Vector2(20, 30);
            rect.X -= rect.Width/2;
            rect.Y -= rect.Height;
            Gui.ShapeSelection.Bounds = rect;
            Add();
          }
        }
        // Dragging
        else if (input.IsDrag) 
        {
          Gui.primitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
          Gui.ShapeSelection.Render(Gui.primitiveBatch, batcher, camera, Editor.ColorSet.AnnotatedShapeActive);
          Gui.primitiveBatch.End(); 
        }
        // Released; add 
        else if (input.IsDragLast && _initialMouse != Vector2.Zero && !(Gui.ShapeSelection is Shape.Polygon)) Add();
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
}
