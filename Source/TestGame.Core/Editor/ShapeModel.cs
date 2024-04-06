
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{ 
  public class ShapeModelUtils
  { 
    public static void RenderShapeModel(object model, PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      if (model is RectangleModel m1) m1.Render(primitiveBatch, batcher, camera, color);
      else if (model is EllipseModel m2) m2.Render(primitiveBatch, batcher, camera, color);
      else if (model is PointModel m3) m3.Render(primitiveBatch, batcher, camera, color);
      else if (model is PolygonModel m4) m4.Render(primitiveBatch, batcher, camera, color);
      batcher.FlushBatch();
    }
    public static bool Collides(object model, Vector2 point)
    { 
      if (model is RectangleModel m1) return m1.Bounds.Contains(point);
      else if (model is EllipseModel m2) return Collisions.CircleToPoint(m2.Center, m2.Width, point);
      else if (model is PointModel m3) return new Nez.PhysicsShapes.Polygon(m3.Vertices).ContainsPoint(point);
      // else if (model is PolygonModel m4) return false; 
      return false;
    }
    public static RectangleF Bounds(object model)
    { 
      if (model is RectangleModel m1) return m1.Bounds;
      else if (model is EllipseModel m2) return new RectangleF(m2.Center.X-m2.Width/2, m2.Center.Y-m2.Width/2, m2.Width, m2.Width);
      else if (model is PointModel m3) return RectangleF.RectEncompassingPoints(m3.Vertices);
      // else if (model is PolygonModel m4) return  
      throw new Exception();
    }
    public static bool IsShape(object shape) =>
         shape is RectangleModel
      || shape is PointModel
      || shape is PolygonModel
      || shape is EllipseModel;

    public readonly static object[] ModelsName = new string[] {
        "Rectangle",
        "Ellipse",
        "Point",
        "Polygon",
       };
    public readonly static string[] ModelsIcon = new string[] {
         IconFonts.FontAwesome5.SquareFull,
         IconFonts.FontAwesome5.Circle,
         IconFonts.FontAwesome5.MapMarkedAlt,
         IconFonts.FontAwesome5.DrawPolygon
       };
    
    public static object[] ModelsInstance { get =>
       new object[] {
        new RectangleModel(),
        new EllipseModel(),
        new PointModel(),
        new PolygonModel()
      };
    }
  }
  public class RectangleModel
  {  
    public RectangleF Bounds = new RectangleF();

    public RectangleModel() {}

    public void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawRectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, color);
      batcher.DrawRectOutline(camera, Bounds, color);
    } 
  }
  public class EllipseModel
  {
    public Vector2 Center = new Vector2();
    public float Width = 0;
    public float Height = 0;

    public EllipseModel() {}

    public void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawCircle(Center, Width, color, circleSegments: (int)(32*camera.RawZoom));
      batcher.DrawCircle(Center, Width, color, thickness: 1/camera.RawZoom, resolution: (int)(32*camera.RawZoom));
    } 
  }

  public class PointModel 
  {
    public static Vector2 Size = new Vector2(20, 30);
    public Vector2 Position = new Vector2();
    public Vector2[] Vertices { get {
      var left = Position;
      left.X -= Size.X/2;
      left.Y -= Size.Y/2;

      var right = Position;
      right.X += Size.X/2;
      right.Y -= Size.Y/2;

      return new []{ left, right, Position };
    }}

    public PointModel() {}

    public void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      var vertices = Vertices;
      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(vertices), vertices.Count(), color);
      batcher.DrawPolygon(Position, vertices, color,  true, 1/camera.RawZoom);
    } 
  }
  public class PolygonModel
  { 
    public List<Vector2> Points = new List<Vector2>();
    public Vector2 Position = new Vector2();

    public PolygonModel() {}

    public void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(Points), Points.Count(), color);
      batcher.DrawPolygon(Position, Points.ToArray(), color, thickness: 1/camera.RawZoom);
    }       
  }
}
