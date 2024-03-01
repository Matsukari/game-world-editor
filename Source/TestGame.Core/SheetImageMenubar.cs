
using ImGuiNET;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetImageControl : Control
    {
      void Annotate(ShapeType shape)
      {
        Gui.ShapeSelection = shape;
        Editor.Set(EditingState.AnnotateShape);
      }
      void HandleShapeAnnotation()
      {
        var rect = Gui.MouseDragArea;
        if (Gui.IsDrag)
        {
          switch (Gui.ShapeSelection)
          {
            case ShapeType.Circle: break;
            case ShapeType.Rectangle: 
              ImUtils.DrawRectFilled(ImGui.GetWindowDrawList(), rect, Editor.ColorSet.AnnotatedShape, Gui.SheetPosition, Gui.ContentZoom);
              break;
            case ShapeType.Ellipse: break;
          }
        }
        else if (Gui.IsDragLast)
        {
          switch (Gui.ShapeSelection)
          {
            case ShapeType.Circle | ShapeType.Rectangle | ShapeType.Ellipse:
              break;
            case ShapeType.Polygon: break;
          }
        }
      }
      void DrawMenubar()
      {
        var newAtlas = false;
        var openFile = false;

        HandleShapeAnnotation();
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
            Annotate(ShapeType.Rectangle);
          }
          if (ImGui.Button("Circle")) 
          {

          }
          if (ImGui.Button("Point"))
          {

          }
          if (ImGui.Button("Polygon"))
          {

          }

          ImGui.EndMenuBar();
        }
        if (newAtlas) ImGui.OpenPopup("new-atlas");
        if (openFile) ImGui.OpenPopup("open-file");
        Gui.LoadTextureFromFilePopup();
      }
    }
  }
}
