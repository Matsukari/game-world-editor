using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven
{
  /// <summary>
  /// Composed of Layers of the same size as this.
  /// </summary>
  public class Level : IPropertied, ICloneable
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public string Name;

    /// <summary>
    /// The size of the Level, inherited by all owned Layers
    /// </summary>
    public Point ContentSize 
    {
      get => _contentSize;
      set 
      {
        var old = _contentSize;
        _contentSize = value;
      }
    }

    [JsonInclude]
    Point _contentSize = new Point(Screen.Width, Screen.Height); 

    /// <summary>
    /// World where this Level is attached
    /// </summary>
    public World World;

    /// <summary>
    /// Actual content
    /// </summary>
    public List<Layer> Layers = new List<Layer>();

    /// <summary>
    /// Position offset from World
    /// </summary>
    public Vector2 LocalOffset = Vector2.Zero;

    /// <summary>
    /// World position plus LocalOffset
    /// </summary>
    public Vector2 Position { get => World.Position + LocalOffset; }

    /// <summary>
    /// Determines whether to draw this Level or not
    /// </summary>
    public bool IsVisible = true;

    /// <summary>
    /// Absolute bounds of the Level
    /// </summary>
    public RectangleF Bounds { get => new RectangleF(
        Position.X,
        Position.Y,
        ContentSize.X,
        ContentSize.Y);
    }

    internal Level()
    {
    }

    public Level(World world)
    {
      World = world;
    }

    public void ReBounds(RectangleF bounds, SelectionAxis axis, Vector2 delta)
    {
      var oldBounds = Bounds;
      LocalOffset = bounds.Location;
      ContentSize = bounds.Size.ToPoint();

      if (axis == SelectionAxis.None) return;
      foreach (var layer in Layers)
      {
        if ((axis == SelectionAxis.TopLeft 
            || axis == SelectionAxis.Top 
            || axis == SelectionAxis.Left) 
            && delta.EitherIsNegative())
        {
          Console.WriteLine("Pushed");
          layer.OnLevelPushed(oldBounds);
        }
        else if (axis == SelectionAxis.TopLeft
              || axis == SelectionAxis.Top
              || axis == SelectionAxis.Left
              || (axis == SelectionAxis.BottomLeft && delta.EitherIsNegative()) 
              || (axis == SelectionAxis.TopRight && delta.EitherIsNegative())
              || (axis == SelectionAxis.BottomRight && delta.EitherIsNegative())
              || (axis == SelectionAxis.Right && delta.X < 0)
              || (axis == SelectionAxis.Bottom && delta.Y < 0) )
              {
                Console.WriteLine("Cutoff");
                if (axis.HasTopOrLeft())
                  layer.OnLevelPushed(oldBounds);

                if (axis.IsDoubleDirection())
                {
                  var (a, b) = axis.SeparateDouble();
                  layer.OnLevelCutoff(a, a.DirectionFactor().ToVector2() * delta);
                  layer.OnLevelCutoff(b, b.DirectionFactor().ToVector2() * delta);
                }
                else 
                {
                  delta.X = Math.Abs(delta.X);
                  delta.Y = Math.Abs(delta.Y);
                  layer.OnLevelCutoff(axis, delta);
                }
              }

      }

    }

    /// <summary>
    /// Adds a layer at the forefront
    /// </summary>
    public void AddLayer(Layer layer)
    {
      Layers.Add(layer);
      Layers = Layers.EnsureNoRepeatNameField();
    }

    /// <summary>
    /// Remoes the layer with the given name
    /// </summary>
    public void RemoveLayer(string name) => Layers.Remove(Layers.Find(item => item.Name == name));

    /// <summary>
    /// Remoes the layer 
    /// </summary>
    public void RemoveLayer(Layer layer) => RemoveLayer(layer.Name);


    /// <summary>
    /// Pushes the given layer at the given index
    /// </summary>
    public void OrderAt(Layer layer, int index) 
    {
      try 
      {
        var current = (Layers.FindIndex(item => item.Name == layer.Name));

        var temp = Layers[index];
        Layers[index] = Layers[current];
        Layers[current] = temp;
      }
      catch (Exception) {} 
    }
    public void BringDown(Layer layer) => OrderAt(layer, Layers.FindIndex(item => item.Name == layer.Name) - 1);

    public void BringUp(Layer layer) => OrderAt(layer, Layers.FindIndex(item => item.Name == layer.Name) + 1); 


    /// <summary>
    /// Remoes this layer from its parent World; the World can no longer refenrece this level but 
    /// this level could in reverse
    /// </summary>
    public void DetachFromWorld() 
    {
      if (World != null)
        World.RemoveLevel(this);
    }

    /// <summary>
    /// Creates a full copy of this Level with each new transform and layers.
    /// </summary>
    public Level Copy() 
    {
      Level level = MemberwiseClone() as Level;
      level.Properties = Properties.Copy();
      level.Layers = Layers.CloneItems();
      for (int i = 0; i < level.Layers.Count(); i++) level.Layers[i].Level = level;
      level.Properties = Properties.Copy();
      return level;
    }

    object ICloneable.Clone() => Copy();

  }
}
