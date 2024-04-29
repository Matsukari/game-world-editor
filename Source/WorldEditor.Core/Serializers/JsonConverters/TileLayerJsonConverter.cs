
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
      for (int y = 0; y < instance.TilesQuantity.Y; y++)
      {
        for (int x = 0; x < instance.TilesQuantity.X; x++)
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

      encoder.EncodeKeyValuePair(instance.Level.Name+instance.Name+"TileWorld", tiles);
    }
    public override void OnFoundCustomData(TileLayer instance, string key, object value )
    {
      Console.WriteLine("OnFoundCustomData TileLayer: " + key);
      Console.WriteLine("value TileLayer: " + value);      
      JsonCache.Data.Add(key, value);
    }
  }
}

