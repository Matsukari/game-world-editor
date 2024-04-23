
using Nez.Persistence;

namespace Raven.Serializers
{

  class TileInstanceJsonConverter : JsonTypeConverter<TileInstance>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, TileInstance instance)
    {
      if (instance.Name != string.Empty)
        encoder.EncodeKeyValuePair("Name", instance.Name);

      if (instance.Properties.Data.Count() > 0)
        encoder.EncodeKeyValuePair("Properties", instance.Properties);

      if (instance.Props != null) 
        encoder.EncodeKeyValuePair("Props", instance.Props);

      encoder.EncodeKeyValuePair("Tile", instance.Tile);
    }
    public override void OnFoundCustomData(TileInstance instance, string key, object value )
    {
    }
  }
}

