
using Nez;
using Microsoft.Xna.Framework;
using Icons = IconFonts.FontAwesome5;
using ImGuiNET;

namespace Raven
{ 
  public abstract class ShapeModel : ICloneable
  {
    public abstract string Icon { get; }
    
    public abstract RectangleF Bounds { get; set; }

    public abstract void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color);

    public virtual void Render(ImDrawListPtr drawlist, Color color) {}

    public abstract bool CollidesWith(Vector2 point);

    public abstract object Duplicate();

    object ICloneable.Clone() => Duplicate();
  }
  public class RectangleModel : ShapeModel
  {  
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.SquareFull; }

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawRectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, color);
      batcher.DrawRectOutline(camera, Bounds, color);
    }

    public override void Render(ImDrawListPtr drawlist, Color color)
    {
      drawlist.AddRectFilled(Bounds.Location.ToNumerics(), Bounds.Max.ToNumerics(), color.ToImColor());
    }

    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();

  }
  public class EllipseModel : ShapeModel
  {
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.Circle; }

    public Vector2 Center { get => Bounds.Center; }
    public float Width { get => Bounds.Width; }
    public float Height { get => Bounds.Height; }

    public EllipseModel() {}

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawCircle(Center, Width, color, circleSegments: (int)(32*camera.RawZoom));
      batcher.DrawCircle(Center, Width, color, thickness: 1/camera.RawZoom, resolution: (int)(32*camera.RawZoom));
    }

    public override void Render(ImDrawListPtr drawlist, Color color)
    {
      ImGuiUtils.DrawEllipseFilled(drawlist, Bounds, color.ToImColor());
      ImGuiUtils.DrawEllipse(drawlist, Bounds, color.ToImColor());
    }

    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();

  }

  public class PointModel : ShapeModel 
  {
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.MapMarkerAlt; }

    public static Vector2 Size = new Vector2(20, 30);
    public Vector2 Position { get => Bounds.Location; }
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

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      var vertices = Vertices;
      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(vertices), vertices.Count(), color);
      batcher.DrawPolygon(Position, vertices, color,  true, 1/camera.RawZoom);
    } 
    public override void Render(ImDrawListPtr drawlist, Color color)
    {
      
    }
    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();

  }
  public class PolygonModel : ShapeModel
  { 
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.DrawPolygon; }

    public List<Vector2> Points = new List<Vector2>();
    public Vector2 Position = new Vector2();

    public PolygonModel() {}

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(Points), Points.Count(), color);
      batcher.DrawPolygon(Position, Points.ToArray(), color, thickness: 1/camera.RawZoom);
    }       

    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();
  }
}
