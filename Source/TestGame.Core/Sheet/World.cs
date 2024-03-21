using Raven.Sheet.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  // <summary>
  // Expands as the levels are painted on
  // </summary>
  public class World : Entity
  {
    public Point Size 
    { 
      get 
      { 
        var size = new Point();
        foreach (var level in Levels) 
          size += level.ContentSize;
        return size;
      } 
    }
    public Level CurrentLevel = null;
    public List<Level> Levels { get => Components.GetComponents<Level>(); } 
    public Dictionary<string, Sheet> SpriteSheets = new Dictionary<string, Sheet>();

    public World() 
    {
      Level level = CreateLevel();
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
    public void AddSheet(Sheet sheet) { SpriteSheets.Add(sheet.Name, sheet); }
    public void DrawArtifacts(Batcher batcher, Camera camera, Editor editor, GuiData gui)
    {
      if (CurrentLevel != null)
      {
        batcher.DrawRectOutline(camera, CurrentLevel.Bounds, editor.ColorSet.SpriteRegionActiveOutline);
      }
    }
  }
}

