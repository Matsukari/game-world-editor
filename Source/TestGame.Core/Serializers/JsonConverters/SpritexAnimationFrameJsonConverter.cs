
using Nez.Persistence;
using Raven.Sheet;

namespace Raven.Serializers
{

  class SpritexAnimationFrameJsonConverter : JsonTypeConverter<SpritexAnimationFrame>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, SpritexAnimationFrame instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Parts", instance.Parts);
      encoder.EncodeKeyValuePair("Duration", instance.Duration);
      encoder.EncodeKeyValuePair("EaseType", instance.EaseType);
    }
    public override void OnFoundCustomData(SpritexAnimationFrame instance, string key, object value )
    {
    }
  }
}

