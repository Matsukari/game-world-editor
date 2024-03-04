
using Nez;

namespace Raven.Sheet
{
  // public enum ShapeType { Circle, Rectangle, Ellipse, Point, Polygon, None };
  public abstract class Shape : IPropertied
  {
    public string Name { get; set; } = "";
    public PropertyList Properties { get; set; } = new PropertyList();
    public RectangleF Bounds { get; set; } = new RectangleF();
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
