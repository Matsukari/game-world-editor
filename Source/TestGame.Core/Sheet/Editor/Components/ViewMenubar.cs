
using ImGuiNET;
using Nez;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetImageControl : Control
    {
      void Annotate(Shape shape)
      {
        Gui.ShapeSelection = shape;
        Editor.Set(EditingState.AnnotateShape);
      }
      void HandleShapeAnnotation(ProppedObject context)
      {
        var rect = Gui.MouseDragArea;
        // rect.X /= Gui.ContentZoom;
        // rect.Y /= Gui.ContentZoom;
        if (Gui.IsDrag && Gui.ShapeSelection != null)
        {
          if (Gui.ShapeSelection is Shape.Rectangle)
              ImGui.GetWindowDrawList().AddRectFilled(rect.Location.ToNumerics(), rect.Max.ToNumerics(), Editor.ColorSet.AnnotatedShapeActive.ToImColor());
          else if (Gui.ShapeSelection is Shape.Circle)
              ImGui.GetWindowDrawList().AddCircleFilled(rect.Center.ToNumerics(), rect.Width/2, Editor.ColorSet.AnnotatedShapeActive.ToImColor());
        }
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
            rect.Location /= Gui.ContentZoom;
            rect.Size /= Gui.ContentZoom;
            Gui.ShapeSelection.Bounds = rect;
            context.Properties.Add(Gui.ShapeSelection);
          }
          Gui.ShapeSelection = null;
        }
      }
      void DrawMenubar()
      {
        var newAtlas = false;
        var openFile = false;

        HandleShapeAnnotation(Gui.ShapeContext);
        if (ImGui.BeginMenuBar())
        {
          if (ImGui.BeginMenu("File"))
          {
            if (ImGui.MenuItem("New Atlas from Folder")) newAtlas = true;
            if (ImGui.MenuItem("Load Atlas or PNG")) openFile = true;
            ImGui.EndMenu();
          }
          if (ImGui.Button("Rectangle"))
          {
            Annotate(new Shape.Rectangle());
          }
          else if (ImGui.Button("Circle")) 
          {
            Annotate(new Shape.Circle());
          }
          else if (ImGui.Button("Point"))
          {
            Annotate(new Shape.Point());
          }
          // if (ImGui.Button("Polygon"))
          // {
          //   Annotate(new Shape.Rectangle());
          // }

          ImGui.EndMenuBar();
        }
        if (newAtlas) ImGui.OpenPopup("new-atlas");
        if (openFile) ImGui.OpenPopup("open-file");
        Gui.LoadTextureFromFilePopup();
      }
    }
  }
}
