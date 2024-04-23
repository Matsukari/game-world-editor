


namespace Raven.Serializers
{
  public class WorldSerializer : JsonSerializer<World>
  {
    protected override World BuildFrom(World model, Dictionary<string, object> cache)
    {
      if (cache["SheetSources"] is List<object> sheetSources) 
      {
        foreach (var sheet in sheetSources)
        {
          model.Sheets.Add(Serializer.LoadContent<Sheet>(sheet as string));
        } 
      }
      return model;
    }
        
    protected override World Realize(World model)
    {
      foreach (var level in model.Levels)
      {
        foreach (var layer in level.Layers)
        {
          if (layer is TileLayer tileLayer)
          {

          }
        }
      }
      return model;
    }
  }
}
