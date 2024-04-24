


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

      foreach (var level in model.Levels)
      {
        foreach (var layer in level.Layers)
        {
          if (layer is TileLayer tileLayer)
          {
            object datHolder;
            if (cache.TryGetValue(level.Name+layer.Name+"TileWorld", out datHolder))
            {
              var serializedTiles = datHolder as string;
              var tiles = serializedTiles.Split(' ');
              Console.WriteLine("Passed, got: " + tiles.Count());
              Console.WriteLine("Tileayer quantiy: " + tileLayer.TilesQuantity.X * tileLayer.TilesQuantity.Y);
              for (int i = 0; i < tiles.Count(); i++)
              {
                var tile = tiles[i];
                var items = tile.Split(',');
                var sheetIndex = Int32.Parse(items[0]);
                var tileIndex = Int32.Parse(items[1]);

                try 
                {
                  if (tileIndex != -1 && sheetIndex != -1)
                  {
                    // Console.WriteLine($"Got: {sheetIndex},{tileIndex}");
                    var pos = tileLayer.GetTile(i);
                    var tileRef = tileLayer.World.Sheets[sheetIndex].GetTileData(tileIndex);
                    // Console.WriteLine("Replacing..");
                    tileLayer.ReplaceTile(pos, tileRef);
                  }
                }
                catch (Exception)
                {
                  Console.WriteLine($"Ignored {sheetIndex},{tileIndex}");
                }
              }
            }
          }
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
