
using Nez;
using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public class Annotator : Editor.SubEntity
  {
    public void Annotate(Shape shape)
    {
      if (Gui.ShapeContext == null) throw new MissingFieldException();
      Gui.ShapeSelection = shape;
      Editor.Set(Editor.EditingState.AnnotateShape);
    }
    public override void OnAddedToScene()
    {
      AddComponent(new Renderable());
    }
    public class Renderable : Editor.SubEntity.RenderableComponent<Annotator>
    {
      Vector2 _initialMouse = Vector2.Zero;
      public override void Render(Batcher batcher, Camera camera)
      {
        // Highlight context's shape proeprties
        DrawPropertiesShapes(Gui.ShapeContext, batcher, camera, Editor.ColorSet.AnnotatedShapeActive);

        if (Editor.EditState != Editor.EditingState.AnnotateShape) return;
        var input = Core.GetGlobalManager<Raven.Input.InputManager>();
    

        var rect = input.MouseDragArea;
        rect.Location = _initialMouse;
        rect.Size = camera.MouseToWorldPoint() - _initialMouse;
        Gui.ShapeSelection.Bounds = rect;
        if (input.IsDragFirst && _initialMouse == Vector2.Zero)
        {
          _initialMouse = camera.MouseToWorldPoint();
        }
        else if (input.IsDrag) Gui.ShapeSelection.Render(batcher, camera, Editor.ColorSet.AnnotatedShapeActive);
        else if (input.IsDragLast && _initialMouse != Vector2.Zero)
        {
          if (Gui.ShapeSelection is Shape.Point)
          {
            rect.Size = new Vector2(20, 30);
            Gui.ShapeSelection.Bounds = rect;
          }
          Gui.ShapeContext.Properties.Add(Gui.ShapeSelection);
          Editor.Set(Editor.EditingState.Default);
          Gui.ShapeSelection = null;
          _initialMouse = Vector2.Zero;
        }
      }
      public static void DrawPropertiesShapes(IPropertied propertied, Batcher batcher, Camera camera, Color color)
      {
        foreach (var prop in propertied.Properties)
        {
          if (prop.Value is Shape shape) shape.Render(batcher, camera, color);
        }
      }
    }
  }
}
