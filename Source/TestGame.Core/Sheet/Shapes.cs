
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

    public virtual void Render(Batcher batcher, Camera camera, Color color) {}
    public object Clone()
    {
      var shape = System.Convert.ChangeType(System.Activator.CreateInstance(GetType()), GetType()) as Shape;
      shape.Bounds = Bounds;
      return shape;
    }
    
    public Shape() {}

    public class Circle : Shape
    {
      [PropertiedInput("Center")]
      public Vector2 Center 
      { 
        get => Bounds.Location+Bounds.Size/2; 
        set => Bounds = new RectangleF(value.X-Bounds.Size.X/2, value.Y-Bounds.Size.Y/2, Bounds.Width, Bounds.Height);  
      }
      public override void Render(Batcher batcher, Camera camera, Color color)
      {
        batcher.DrawCircle(Bounds.Location+Bounds.Size/2, Bounds.Width/2, color);
      } 

    }
    public class Ellipse : Shape
    { 
      public override void Render(Batcher batcher, Camera camera, Color color)
      {
        batcher.DrawCircle(Bounds.Location+Bounds.Size/2, Bounds.Width/2, color);
      } 
    }
    public class Polygon : Shape
    {  
      public List<Vector2> Points = new List<Vector2>();
      public override void Render(Batcher batcher, Camera camera, Color color)
      {
        batcher.DrawPolygon(Bounds.Location, Points.ToArray(), color);
      }       
    }
    public class Rectangle : Shape
    {  
      public override void Render(Batcher batcher, Camera camera, Color color)
      {
        batcher.DrawRect(Bounds, color);
      } 
    }
    public class Point : Shape
    {
      [PropertiedInput("Position")]
      public Vector2 Position 
      { 
        get => Bounds.BottomCenter(); 
        set => Bounds = new RectangleF(value.X-Bounds.Location.X/2, value.Y-Bounds.Size.Y, Bounds.Width, Bounds.Height);  
      }
      public override void Render(Batcher batcher, Camera camera, Color color)
      {
        batcher.DrawPolygon(Bounds.Location, new []{
            Bounds.Location-Bounds.Location, 
            Bounds.TopRight()-Bounds.Location, 
            Bounds.BottomCenter()-Bounds.Location}, 
            color, 
            true, 
            1/camera.RawZoom);
      } 
    }
  }
}
