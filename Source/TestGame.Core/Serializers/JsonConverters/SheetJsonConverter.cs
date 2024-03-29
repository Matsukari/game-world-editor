using Nez.Persistence;

namespace Raven.Serializers
{

  class SheetJsonConverter : JsonTypeConverter<Sheet.Sheet>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Sheet.Sheet instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Filename", instance.Filename);
      encoder.EncodeKeyValuePair("_tiles", instance._tiles);
      encoder.EncodeKeyValuePair("Spritexes", instance.Spritexes);
      encoder.EncodeKeyValuePair("TileWidth", instance.TileWidth);
      encoder.EncodeKeyValuePair("TileHeight", instance.TileHeight);
    }
    public override void OnFoundCustomData(Sheet.Sheet instance, string key, object value )
    {
    }
  }
}

