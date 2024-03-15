
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
      var com = AddComponent(new Renderable());
      com.RenderLayer = -1;
    }
    public class Renderable : Editor.SubEntity.RenderableComponent<Annotator>
    {
      Vector2 _initialMouse = Vector2.Zero;
      public override void Render(Batcher batcher, Camera camera)
      {

        var input = Core.GetGlobalManager<Raven.Input.InputManager>();
        if (Editor.EditState != Editor.EditingState.AnnotateShape || input.IsImGuiBlocking) return;

        var rect = input.MouseDragArea;
        rect.Location = _initialMouse;
        rect.Size = camera.MouseToWorldPoint() - _initialMouse;
        Gui.ShapeSelection.Bounds = rect;
        
        void Add()
        {
          Gui.ShapeContext.Properties.Add(Gui.ShapeSelection);
          Editor.Set(Editor.EditingState.Default);
          Gui.ShapeSelection = null;
          _initialMouse = Vector2.Zero;
        }
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
        else if (input.IsDrag) 
        {
          Gui.primitiveBatch.Begin(camera.ProjectionMatrix, camera.TransformMatrix);
          Gui.ShapeSelection.Render(Gui.primitiveBatch, batcher, camera, Editor.ColorSet.AnnotatedShapeActive);
          Gui.primitiveBatch.End(); 
        }
        else if (input.IsDragLast && _initialMouse != Vector2.Zero && !(Gui.ShapeSelection is Shape.Polygon)) Add();
      }
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
