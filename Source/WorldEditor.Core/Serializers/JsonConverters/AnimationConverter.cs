
using Nez.Persistence;

namespace Raven.Serializers
{
  class AnimationJsonConverter : JsonTypeConverter<Animation>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Animation instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("IsContinous", instance.IsContinous);
      encoder.EncodeKeyValuePair("CurrentFrame", instance.CurrentFrame);
      encoder.EncodeKeyValuePair("Frames", instance.Frames);
    }
    public override void OnFoundCustomData(Animation instance, string key, object value )
    {
    }
  }
}

