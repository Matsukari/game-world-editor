
using Nez;
using Microsoft.Xna.Framework;

namespace Raven
{ 
  public class RectangleModel
  {  
    public static string Icon = IconFonts.FontAwesome5.SquareFull;
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
    public static string Icon = IconFonts.FontAwesome5.Circle;
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
    public static string Icon = IconFonts.FontAwesome5.MapMarkerAlt;
    public static Vector2 Size = new Vector2(20, 30);
    public Vector2 Position = new Vector2();

    public PointModel() {}

    public void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      var left = Position;
      left.X -= Size.X/2;
      left.Y -= Size.Y/2;

      var right = Position;
      right.X += Size.X/2;
      right.Y -= Size.Y/2;

      var vertices = new []{ left, right, Position };

      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(vertices), vertices.Count(), color);
      batcher.DrawPolygon(Position, vertices, color,  true, 1/camera.RawZoom);
    } 
  }
  public class PolygonModel
  { 
    public static string Icon = IconFonts.FontAwesome5.DrawPolygon;
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
