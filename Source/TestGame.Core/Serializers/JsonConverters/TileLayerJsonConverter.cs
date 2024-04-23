
using Nez.Persistence;

namespace Raven.Serializers
{

  class TileLayerJsonConverter : JsonTypeConverter<TileLayer>
  {
    public override bool WantsExclusiveWrite => true;
    public override bool CanWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, TileLayer instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Level", instance.Level);
      encoder.EncodeKeyValuePair("Offset", instance.Offset);
      encoder.EncodeKeyValuePair("Opacity", instance.Opacity);
      encoder.EncodeKeyValuePair("IsLocked", instance.IsLocked);
      encoder.EncodeKeyValuePair("IsVisible", instance.IsVisible);
      encoder.EncodeKeyValuePair("TileWidth", instance.TileWidth);
      encoder.EncodeKeyValuePair("TileHeight", instance.TileHeight);
 
      string tiles = "";
      for (int x = 0; x < instance.TilesQuantity.X; x++)
      {
        for (int y = 0; y < instance.TilesQuantity.Y; y++)
        {
          var pos = new Microsoft.Xna.Framework.Point(x, y);
          var sheetIndex = -1;
          var tileIndex = -1;
          try 
          {
            var tileInstance = instance.Tiles[pos];
            sheetIndex = instance.Level.World.Sheets.FindIndex(item => item.Name == tileInstance.Tile._sheet.Name);
            if (sheetIndex == -1) throw new Exception("The tile does not reference any sheet as source");
            tileIndex = tileInstance.Tile.Id;
          }
          catch (Exception)
          {
          }
          tiles += ($"{sheetIndex},{tileIndex} ");
        }
      }
      tiles = string.Concat(tiles.SkipLast(1));

      encoder.EncodeKeyValuePair("TileWorld", tiles);
    }
    public override void OnFoundCustomData(TileLayer instance, string key, object value )
    {
      // Console.WriteLine("OnFoundCustomData TileLayer: " + key);
      // Console.WriteLine("value TileLayer: " + value);      
      // if (key == "TileWorld")
      // {
      //   var tiles = JsonCache.Data.Dig(instance.Level.Name);
      //   tiles[instance.Name] = value; 
      //   
      // }
        if (key == "TileWorld" && value is string serializedTiles) 
        {
          var tiles = serializedTiles.Split(' ');
          foreach (var tile in tiles)
          {
            var items = tile.Split(',');
            var sheetIndex = Int32.Parse(items[0]);
            var tileIndex = Int32.Parse(items[1]);
            var pos = instance.GetTile(tileIndex);
            var tileRef = instance.World.Sheets[sheetIndex].GetTileData(tileIndex);
            instance.ReplaceTile(pos, tileRef);
          }
          Console.WriteLine(serializedTiles);
          var tileWorld = Json.FromJson(serializedTiles);
        }

    }
  }
}

