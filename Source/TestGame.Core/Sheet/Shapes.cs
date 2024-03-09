
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  // public enum ShapeType { Circle, Rectangle, Ellipse, Point, Polygon, None };
  public abstract class Shape : ICloneable
  {
    public string Name { get; set; } = "";
    [PropertiedInput("Bounds")]
    public RectangleF Bounds { get; set; } = new RectangleF();
  
    // Must always use primitiveBatch because .begin is called every frame
    public abstract void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color);
    public object Clone()
    {
      var shape = System.Convert.ChangeType(System.Activator.CreateInstance(GetType()), GetType()) as Shape;
      shape.Bounds = Bounds;
      return shape;
    }
    public Shape() {}

    public class Rectangle : Shape
    {  
      public static string Icon = IconFonts.FontAwesome5.SquareFull;
      public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
      {
        primitiveBatch.DrawRectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, color);
        batcher.DrawRectOutline(camera, Bounds, color * 3);
      } 
    }
    public class Circle : Shape
    {
      public static string Icon = IconFonts.FontAwesome5.Circle;
      [PropertiedInput("Center")]
      public Vector2 Center 
      { 
        get => Bounds.Location+Bounds.Size/2; 
        set => Bounds = new RectangleF(value.X-Bounds.Size.X/2, value.Y-Bounds.Size.Y/2, Bounds.Width, Bounds.Height);  
      }
      public float Radius
      { 
        get => Bounds.Width/2; 
        set 
        {
          Bounds = new RectangleF(Center.X - value, Center.Y - value/2, value, value);
        }
      }
      public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
      {
        primitiveBatch.DrawCircle(Center, Radius, color);
        batcher.DrawCircle(Center, Radius, color * 3);
      } 
    }

    public class Point : Shape
    {
      public static string Icon = IconFonts.FontAwesome5.MapMarker;
      [PropertiedInput("Position")]
      public Vector2 Position 
      { 
        get => Bounds.BottomCenter(); 
        set => Bounds = new RectangleF(value.X-Bounds.Location.X/2, value.Y-Bounds.Size.Y, Bounds.Width, Bounds.Height);  
      }
      public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
      {
        primitiveBatch.DrawPolygon(new []{
            Bounds.Location + Bounds.Location-Bounds.Location, 
            Bounds.Location + Bounds.TopRight()-Bounds.Location, 
            Bounds.Location + Bounds.BottomCenter()-Bounds.Location}, 
            3,
            color);
        batcher.DrawPolygon(Bounds.Location, new []{
            Bounds.Location-Bounds.Location, 
            Bounds.TopRight()-Bounds.Location, 
            Bounds.BottomCenter()-Bounds.Location}, 
            color, 
            true,
            2f);

      } 
    }
    public class Polygon : Shape
    { 
      public static string Icon = IconFonts.FontAwesome5.DrawPolygon;
      public List<Vector2> Points = new List<Vector2>();
      public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
      {
        primitiveBatch.DrawPolygon(Vector2Ext.AddPositionVertices(Points, Bounds.Location), Points.Count(), color);
        batcher.DrawPolygon(Bounds.Location, Points.ToArray(), color * 3);
      }       
    }
  }
}
