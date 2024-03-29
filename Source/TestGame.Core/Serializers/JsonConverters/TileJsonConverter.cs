
using Nez.Persistence;
using Raven.Sheet.Sprites;

namespace Raven.Serializers
{

  class TileJsonConverter : JsonTypeConverter<Tile>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Tile instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Coordinates", instance.Coordinates);
    }
    public override void OnFoundCustomData(Tile instance, string key, object value )
    {
    }
  }
}

