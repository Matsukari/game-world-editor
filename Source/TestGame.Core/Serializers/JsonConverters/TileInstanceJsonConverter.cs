
using Nez.Persistence;

namespace Raven.Serializers
{

  class TileInstanceJsonConverter : JsonTypeConverter<TileInstance>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, TileInstance instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Props", instance.Props);
      encoder.EncodeKeyValuePair("Tile", instance.Tile);
    }
    public override void OnFoundCustomData(TileInstance instance, string key, object value )
    {
    }
  }
}

