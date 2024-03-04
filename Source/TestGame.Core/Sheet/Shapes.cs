
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{
  // public enum ShapeType { Circle, Rectangle, Ellipse, Point, Polygon, None };
  public abstract class Shape 
  {
    public string Name { get; set; } = "";
    public RectangleF Bounds { get; set; } = new RectangleF();
    public virtual void Render(Batcher batcher, Camera camera, Color color) {}
    

    public Shape() {}

    public class Circle : Shape
    {  
    }
    public class Ellipse : Shape
    {  
    }
    public class Polygon : Shape
    {  
    }
    public class Rectangle : Shape
    {  
    }
    public class Point : Shape
    {  
    }
  }
}
