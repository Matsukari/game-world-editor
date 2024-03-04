
using ImGuiNET;
using Num = System.Numerics;
using Nez;

namespace Tools 
{
  public partial class SpriteSheetEditor 
  {
    public partial class SheetImageControl : Control
    {
      void DrawOverlays()
      {
        foreach (var prop in Gui.ShapeContext.Properties)
        {
          DrawObject(prop.Value);
          var (min, max) = ImUtils.GetWindowArea();
          var pos = new Num.Vector2();
          if (prop.Value is Shape shape) 
          {
            pos = shape.Bounds.Location.ToNumerics() * Gui.ContentZoom;
            ImGui.GetWindowDrawList().AddText(pos + Gui.SheetPosition + min, Editor.ColorSet.AnnotatedName.ToImColor(), shape.Name);
          }
        }
      }
      void DrawObject(object obj)
      {
        switch (obj)
        {
          case Shape.Rectangle rectangle: 
            ImUtils.DrawRectFilled(ImGui.GetWindowDrawList(), 
                rectangle.Bounds, Editor.ColorSet.AnnotatedShapeInactive, Gui.SheetPosition, Gui.ContentZoom); 
            break;                 

          case Shape.Circle circle: 
            ImUtils.DrawCircleFilled(ImGui.GetWindowDrawList(), 
                circle.Bounds.Center.ToNumerics(), 
                circle.Bounds.Width/2, 
                Editor.ColorSet.AnnotatedShapeInactive, Gui.SheetPosition, Gui.ContentZoom); 
            break;  

          case Shape.Point point: 
            ImUtils.DrawArrowDownFilled(ImGui.GetWindowDrawList(), 
                point.Bounds.Location.ToNumerics(), 30, 
                Editor.ColorSet.AnnotatedShapeInactive, Gui.SheetPosition, Gui.ContentZoom); 
            break;                                        
        }
      }
    }
  }
}
