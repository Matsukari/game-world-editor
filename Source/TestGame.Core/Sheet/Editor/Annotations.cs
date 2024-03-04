
using Nez;

namespace Raven.Sheet
{
  public class Annotator : Editor.SubEntity
  {
    public void Annotate(Shape shape)
    {
      Gui.ShapeSelection = shape;
      Editor.Set(Editor.EditingState.AnnotateShape);
    }
    public override void OnAddedToScene()
    {
      AddComponent(new Renderable());
    }

    public class Renderable : Editor.SubEntity.RenderableComponent<Annotator>
    {
      public override void Render(Batcher batcher, Camera camera)
      {
        foreach (var prop in Gui.ShapeContext.Properties)
        {
          if (prop.Value is Shape shape) shape.Render(batcher, camera, Editor.ColorSet.AnnotatedShapeInactive);
          var rect = Gui.MouseDragArea;
          // rect.X /= Gui.ContentZoom;
          // rect.Y /= Gui.ContentZoom;
          if (Gui.IsDrag && Gui.ShapeSelection is Shape dragShape) dragShape.Render(batcher, camera, Editor.ColorSet.AnnotatedShapeActive);
          else if (Gui.IsDragLast)
          {
            if (Gui.ShapeSelection is Shape.Circle || Gui.ShapeSelection is Shape.Rectangle || Gui.ShapeSelection is Shape.Point)
            {
              rect.X -= ImUtils.GetWindowRect().X;
              rect.Y -= ImUtils.GetWindowRect().Y;
              Console.WriteLine($"{Gui.SheetPosition} - {rect.Location}");
              // rect.Location = Math.Abs(Gui.SheetPosition + rect.Location;
              rect.X = Math.Abs(Gui.SheetPosition.X) + rect.X;
              rect.Y = Math.Abs(Gui.SheetPosition.Y) + rect.Y;
              rect.Location /= Gui.Zoom;
              rect.Size /= Gui.Zoom;
              Gui.ShapeSelection.Bounds = rect;
              Gui.ShapeContext.Properties.Add(Gui.ShapeSelection);
            }
            Gui.ShapeSelection = null;
          }
        }
      }
    }
  }
}
