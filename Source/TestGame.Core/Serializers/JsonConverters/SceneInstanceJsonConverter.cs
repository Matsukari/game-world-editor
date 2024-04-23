
using Nez.Persistence;

namespace Raven.Serializers
{

  class SpriteSceneInstanceJsonConverter : JsonTypeConverter<SpriteSceneInstance>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, SpriteSceneInstance instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Props", instance.Props);
      encoder.EncodeKeyValuePair("SceneSource", instance.Scene.Name);
    }
    public override void OnFoundCustomData(SpriteSceneInstance instance, string key, object value )
    {
    }
  }
}

