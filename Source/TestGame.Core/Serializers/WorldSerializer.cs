

namespace Raven.Serializers
{
  public class WorldSerializer : JsonSerializer<World>
  {
    protected override World Realize(World model)
    {
      foreach (var level in model.Levels)
      {
        
      }
      return model;
    }
  }
}
