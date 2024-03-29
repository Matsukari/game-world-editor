
using Nez.Persistence;
using Raven.Sheet.Sprites;

namespace Raven.Serializers
{

  class SpriteJsonConverter : JsonTypeConverter<Sprite>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Sprite instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Region", instance.Region);
    }
    public override void OnFoundCustomData(Sprite instance, string key, object value )
    {
    }
  }
}

