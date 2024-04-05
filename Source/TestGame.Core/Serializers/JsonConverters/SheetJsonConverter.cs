using Nez.Persistence;

namespace Raven.Serializers
{

  class SheetJsonConverter : JsonTypeConverter<Sheet>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Sheet instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Source", instance.Source);
      encoder.EncodeKeyValuePair("_tiles", instance._tiles);
      encoder.EncodeKeyValuePair("SpriteScenees", instance.SpriteScenees);
      encoder.EncodeKeyValuePair("TileWidth", instance.TileWidth);
      encoder.EncodeKeyValuePair("TileHeight", instance.TileHeight);
    }
    public override void OnFoundCustomData(Sheet instance, string key, object value )
    {
    }
  }
}

