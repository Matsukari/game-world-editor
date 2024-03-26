using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  // <summary>
  // Composed of Levels.
  // </summary>
  public class World : Entity, IPropertied
  {
    string IPropertied.Name { get => Name; set => Name = value; }
    public PropertyList Properties { get; set; } = new PropertyList();

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

    public Level CurrentLevel = null;
    public List<Level> Levels { get => Components.GetComponents<Level>(); } 
    public Dictionary<string, Sheet> SpriteSheets = new Dictionary<string, Sheet>();

    public World() 
    {
      Level level = CreateLevel();
      Name = "Default World";
      CurrentLevel = level;
    }
    public Level GetLevel(string name)
    {
      foreach (var level in Levels) if (level.Name == name) return level;
      return null;
    }
    public void RemoveLevel(Level level) => RemoveComponent(level);
    public void RemoveLevel(string level) => RemoveComponent(GetLevel(level));
 
    public Level CreateLevel(string name = "Level default") 
    {
      Level level = new Level(this);
      Layer layer = new TileLayer(level, 16, 16);
      level.Name = name;
      level.Layers.Add(layer);
      level.CurrentLayer = layer;
      AddComponent(level);
      return level;
    }
    public void AddSheet(Sheet sheet) 
    { 
      SpriteSheets.Add(sheet.Name, sheet); 
    }
  }
}

