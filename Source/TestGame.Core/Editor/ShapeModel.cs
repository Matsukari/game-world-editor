
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

    public virtual void Render(ImDrawListPtr drawlist, Color color, Color color2) => Render(drawlist, Vector2.Zero, 1f, color, color2);

    public virtual void Render(ImDrawListPtr drawlist, Vector2 offset, float zoom, Color color, Color color2) {}

    public virtual void Render(ImDrawListPtr drawlist, Camera camera, Color color, Color color2)
    {
      var temp = Bounds;
      var temp2 = Bounds;
      temp.Location = camera.WorldToScreenPoint(temp.Location);
      temp.Size *= camera.RawZoom;
      Bounds = temp;   
      Render(drawlist, color, color2);
      Bounds = temp2;
    }

    public abstract bool CollidesWith(Vector2 point);

    public abstract object Duplicate();

    object ICloneable.Clone() => Duplicate();
  }
  public class RectangleModel : ShapeModel
  {  
    [PropertiedInput("Bounds")]
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.SquareFull; }

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawRectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, color);
      batcher.DrawRectOutline(camera, Bounds, color);
    }

    public override void Render(ImDrawListPtr drawlist, Vector2 offset, float zoom, Color color, Color color2)      
    {
      var bounds = Bounds;
      bounds.Location *= zoom;
      bounds.Location += offset;
      bounds.Size *= zoom;
      drawlist.AddRectFilled(bounds.Location.ToNumerics(), bounds.Max.ToNumerics(), color.ToImColor());
      drawlist.AddRect(bounds.Location.ToNumerics(), bounds.Max.ToNumerics(), color2.ToImColor());
    }

    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();

  }
  public class EllipseModel : ShapeModel
  {
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.Circle; }

    [PropertiedInput("Center")]
    public Vector2 Center { get => Bounds.Center; set => Bounds = new RectangleF(value-Bounds.Size/2, Bounds.Size); }

    [PropertiedInput("Width")]
    public float Width { get => Bounds.Width; set => Bounds = new RectangleF(Bounds.X, Bounds.Y, value, Bounds.Height); }

    [PropertiedInput("Height")]
    public float Height { get => Bounds.Height; set => Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, value); }

    public EllipseModel() {}

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawCircle(Center, Width, color, circleSegments: (int)(32*camera.RawZoom));
      batcher.DrawCircle(Center, Width, color, thickness: 1/camera.RawZoom, resolution: (int)(32*camera.RawZoom));
    }

    public override void Render(ImDrawListPtr drawlist, Vector2 offset, float zoom, Color color, Color color2)      
    {
      var bounds = Bounds;
      bounds.Location *= zoom;
      bounds.Location += offset;
      bounds.Size *= zoom;
      ImGuiUtils.DrawEllipseFilled(drawlist, bounds, color.ToImColor());
      ImGuiUtils.DrawEllipse(drawlist, bounds, color2.ToImColor());
    }

    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();

  }

  public class PointModel : ShapeModel 
  {
    public override RectangleF Bounds { get; set; } = new RectangleF();
    public override string Icon { get => Icons.MapMarkerAlt; }

    public static Vector2 Size = new Vector2(20, 30);

    [PropertiedInput("Position")]
    public Vector2 Position { get => Bounds.Location; set => Bounds = new RectangleF(value-Bounds.Size/2, Bounds.Size); }

    public PointModel() {}

    public Vector2[] GetVertices(Vector2 pos)
    {
      var left = pos;
      left.X -= Size.X/2;
      left.Y -= Size.Y;

      var right = pos;
      right.X += Size.X/2;
      right.Y -= Size.Y;

      return new []{ left, right, pos };
    }

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      var vertices = GetVertices(Position);
      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(vertices), vertices.Count(), color);
      batcher.DrawPolygon(Position, vertices, color,  true, 1/camera.RawZoom);
    } 
    public override void Render(ImDrawListPtr drawlist, Vector2 offset, float zoom, Color color, Color color2)      
    {
      var bounds = Bounds;
      bounds.Location *= zoom;
      bounds.Location += offset;
      bounds.Size *= zoom;
      var vertices = GetVertices(bounds.Location);
      drawlist.AddTriangleFilled(vertices[0].ToNumerics(), vertices[1].ToNumerics(), vertices[2].ToNumerics(), color.ToImColor());
      drawlist.AddTriangle(vertices[0].ToNumerics(), vertices[1].ToNumerics(), vertices[2].ToNumerics(), color2.ToImColor());
    }
    public override bool CollidesWith(Vector2 point) => Bounds.Contains(point);

    public override object Duplicate() => MemberwiseClone();

  }
  public class PolygonModel : ShapeModel
  { 
    public override RectangleF Bounds { get; set; } = new RectangleF(); 
    public override string Icon { get => Icons.DrawPolygon; }

    [PropertiedInput("Points")]
    public List<Vector2> Points { get; set; } = new List<Vector2>();

    [PropertiedInput("Position")]
    public Vector2 Position { get => Bounds.Location; set => Bounds = new RectangleF(value, Bounds.Size); }

    public PolygonModel() {}

    public override void Render(PrimitiveBatch primitiveBatch, Batcher batcher, Camera camera, Color color)
    {
      primitiveBatch.DrawPolygon(Position.AddAsPositionToVertices(Points), Points.Count(), color);
      batcher.DrawPolygon(Position, Points.ToArray(), color, thickness: 1/camera.RawZoom);
    }       
    public override void Render(ImDrawListPtr drawlist, Vector2 offset, float zoom, Color color, Color color2)      
    {
      Vector2 Transform(Vector2 pos)
      {
        pos += Position;
        pos *= zoom;
        pos += offset;
        return pos; 
      }
      for (int i = 0; i < Points.Count()-1; i++)
      {
        var a = Transform(Points[i]);
        var b = Transform(Points[i+1] );
        // Console.WriteLine($"Redering point {i} at {a}");
        drawlist.AddLine(a.ToNumerics(), b.ToNumerics(), color2.ToImColor());
      }
      for (int i = 0; i < Points.Count(); i++)
        drawlist.AddCircleFilled(Transform(Points[i] ).ToNumerics(), 4, color2.ToImColor()); 
    }
    public override void Render(ImDrawListPtr drawlist, Camera camera, Color color, Color color2)
    {
      Vector2 Transform(Vector2 pos)
      {
        return camera.WorldToScreenPoint(pos); 
      }
      for (int i = 0; i < Points.Count()-1; i++)
      {
        var a = Transform(Points[i]);
        var b = Transform(Points[i+1] );
        // Console.WriteLine($"Redering point {i} at {a}");
        drawlist.AddLine(a.ToNumerics(), b.ToNumerics(), color2.ToImColor());
      }
      for (int i = 0; i < Points.Count(); i++)
        drawlist.AddCircleFilled(Transform(Points[i] ).ToNumerics(), 4, color2.ToImColor()); 
    }

    public override bool CollidesWith(Vector2 point) 
    {
      Nez.CollisionResult res;
      return Nez.PhysicsShapes.ShapeCollisions.PointToPoly(point, new Nez.PhysicsShapes.Polygon(Points.ToArray()), out res);
    }

    public override object Duplicate()
    {
      var poly = MemberwiseClone() as PolygonModel;
      poly.Points = Points.Copy();
      return poly;
    }
      
  }
}
