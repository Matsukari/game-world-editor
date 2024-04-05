
using Nez.Persistence;

namespace Raven.Serializers
{

  class SpriteSceneAnimationFrameJsonConverter : JsonTypeConverter<SpriteSceneAnimationFrame>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, SpriteSceneAnimationFrame instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Parts", instance.Parts);
      encoder.EncodeKeyValuePair("Duration", instance.Duration);
      encoder.EncodeKeyValuePair("EaseType", instance.EaseType);
    }
    public override void OnFoundCustomData(SpriteSceneAnimationFrame instance, string key, object value )
    {
    }
  }
}

