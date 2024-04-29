
using Nez.Persistence;

namespace Raven.Serializers
{

  class SpriteSceneJsonConverter : JsonTypeConverter<SpriteScene>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, SpriteScene instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Parts", instance.Parts);
      encoder.EncodeKeyValuePair("Animations", instance.Animations);
      encoder.EncodeKeyValuePair("Transform", instance.Transform);
    }
    public override void OnFoundCustomData(SpriteScene instance, string key, object value )
    {
    }
  }
}

