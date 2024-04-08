using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Persistence;

namespace Raven
{
  public class WorldEntity : Entity
  {
    public readonly World World;
    public List<LevelEntity> Levels = new List<LevelEntity>();
    public WorldEntity(World world) => World = world;
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

  public class WorldRenderer
  {
    public static void RenderLayer(Batcher batcher, Camera camera, Layer layer)
    {
      if (layer is TileLayer tileLayer && layer.IsVisible)
      {
        foreach (var (tilePosition, tile) in tileLayer.Tiles)
        {
          var dest = new RectangleF(
              tilePosition.X*tileLayer.TileWidth, 
              tilePosition.Y*tileLayer.TileHeight, 
              tileLayer.TileWidth, tileLayer.TileHeight);

          dest.Location += tileLayer.Bounds.Location;

          var scale = Vector2.One;
          scale.X *= dest.Width / tile.Region.Width;
          scale.Y *= dest.Height / tile.Region.Height;

          var rot = 0f;
          var eff = SpriteEffects.None;
          var color = Color.White;

          RenderProperties renderProp;
          if (tileLayer.TilesProp.TryGetValue(tilePosition, out renderProp)) 
          {
            rot *= renderProp.Transform.RotationRadians;
            eff = renderProp.SpriteEffects;
            color = renderProp.Color.ToColor();
          }

          batcher.Draw(
              texture: tile.Texture,
              position: dest.Location,
              sourceRectangle: tile.Region,
              color: color,
              rotation: rot,
              origin: Vector2.Zero,
              scale: scale,
              effects: eff,
              layerDepth: 0);
        }
      }
    }
    public static void Render(Batcher batcher, Camera camera, World world) 
    {
      foreach (var level in world.Levels) 
      {
        foreach (var layer in level.Layers)
        {
          RenderLayer(batcher, camera, layer);
        }
      }
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

    /// <summary>
    /// Absolute position of the World affecting all objects in the hierarchy
    /// </summary>
    public Vector2 Position = Vector2.Zero;

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
      Levels.Add(level);
      return level;
    }

    /// <summary>
    /// Adds an exisiting Level without or with a parent to this World. 
    /// If given level is attached to a world, then remove it from that world 
    /// and put it here.
    /// </summary>
    public void PutLevel(Level level)
    {
      level.DetachFromWorld();
      Levels.Add(level);
    }
    /// <summary>
    /// Adds a Sheet resource
    /// </summary>
    public void AddSheet(Sheet sheet) => Sheets.Add(sheet); 
  }
}

