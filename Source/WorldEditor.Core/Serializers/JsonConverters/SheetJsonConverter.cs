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
      encoder.EncodeKeyValuePair("Animations", instance.Animations);
      encoder.EncodeKeyValuePair("Source", instance.Source);
      encoder.EncodeKeyValuePair("Tiles", instance.Tiles);
      encoder.EncodeKeyValuePair("SpriteScenees", instance.SpriteScenees);
      encoder.EncodeKeyValuePair("TileWidth", instance.TileWidth);
      encoder.EncodeKeyValuePair("TileHeight", instance.TileHeight);
    }
    public override void OnFoundCustomData(Sheet instance, string key, object value )
    {
    }
  }
}

