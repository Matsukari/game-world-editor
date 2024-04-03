using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence;

namespace Raven
{
  public class WorldEntity : Entity
  {
    public readonly World World;
    public List<LevelEntity> Levels = new List<LevelEntity>();

    /// <summary>
    /// Bounds containing all Levels
    /// </summary>
    public RectangleF Bounds 
    { 
      get 
      {
        var min = new Vector2(100000, 100000);
        var max = new Vector2(-10000, -10000);
        foreach (var level in Levels)
        {
          min.X = Math.Min(min.X, level.Bounds.X);
          min.Y = Math.Min(min.Y, level.Bounds.Y);
          max.X = Math.Max(max.X, level.Bounds.Right);
          max.Y = Math.Max(max.Y, level.Bounds.Bottom);
        }
        return RectangleF.FromMinMax(min, max);
      }
    }
    public void RemoveLevel(Level level)
    {
      World.RemoveLevel(level);
      var index = Levels.FindIndex(item => item.Level.Name == level.Name);
      if (index != -1)
      {
        Levels[index].DetachFromScene();
        Levels.RemoveAt(index);
      } 
    }
    public LevelEntity CreateLevel(string name) 
    {
      Level level = World.CreateLevel(name);
      var levelEntity = new LevelEntity(level);
      levelEntity.SetParent(this);
      Levels.Add(levelEntity); 
      return levelEntity;
    }
  }
  /// <summary>
  /// Composed of interconnected Levels. This is where all you should dump topdwon oriented rendering.
  /// </summary>
  public class World : IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    /// <summary>
    /// ID used for indentidying World
    /// </summary>
    public string Name;

    /// <summary>
    /// The content 
    /// </summary>
    public List<Level> Levels = new List<Level>();

    /// <summary>
    /// The only Sheets Levels can use
    /// </summary>
    public List<Sheet> Sheets = new List<Sheet>();



    public World() 
    {
      Name = Path.Combine(System.Environment.CurrentDirectory, "Untitled.rvworld").GetUniqueFileName();
      Level level = CreateLevel();
    }

    /// <summary>
    /// Finds the last added Level attached to this World with the given Name
    /// </summary>
    public Level GetLevel(string name) => Levels.FindLast(item => item.Name == name);

    /// <summary>
    /// Removes the given level contained in this World
    /// </summary>
    public void RemoveLevel(Level level) => Levels.Remove(level);

    /// <summary>
    /// Creates a blank Level with the given name
    /// </summary>
    public Level CreateLevel(string name = "Level") 
    {
      if (Levels.Find(item => item.Name == name) != null) 
        name = name.EnsureNoRepeat();

      Level level = new Level(this);
      Layer layer = new TileLayer(level, 16, 16);
      level.Name = name;
      level.Layers.Add(layer);
      return level;
    }

    /// <summary>
    /// Adds a Sheet resource
    /// </summary>
    public void AddSheet(Sheet sheet) => Sheets.Add(sheet); 
  }
}

