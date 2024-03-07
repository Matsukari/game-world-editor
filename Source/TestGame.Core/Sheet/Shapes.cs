
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  // public enum ShapeType { Circle, Rectangle, Ellipse, Point, Polygon, None };
  public abstract class Shape : ICloneable
  {
    public string Name { get; set; } = "";
    public RectangleF Bounds { get; set; } = new RectangleF();
    public virtual void Render(Batcher batcher, Camera camera, Color color) {}
    public object Clone()
    {
      return System.Activator.CreateInstance(GetType());
    }
    
    public Shape() {}

    public class Circle : Shape
    {
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
