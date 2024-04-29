using Nez;

namespace Raven 
{
  public class WorldEntity : Entity
  {
    public readonly World World;
    public List<LevelEntity> Levels = new List<LevelEntity>();
    public WorldEntity(World world) 
    {
      World = world;
    }
    public override void OnAddedToScene()
    {
      foreach (var level in World.Levels)
      {
        var levelEntity = new LevelEntity(level);
        levelEntity.SetParent(this);
        Scene.AddEntity(levelEntity);
        Levels.Add(levelEntity);
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
      Scene.AddEntity(levelEntity);
      Levels.Add(levelEntity); 
      return levelEntity;
    }
  }
}
