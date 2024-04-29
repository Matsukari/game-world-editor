
using Nez.Persistence;

namespace Raven.Serializers
{

  class AnimatedSpriteJsonConverter : JsonTypeConverter<AnimatedSprite>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, AnimatedSprite instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("IsVisible", instance.IsVisible);
      encoder.EncodeKeyValuePair("Color", instance.Color);
      encoder.EncodeKeyValuePair("Origin", instance.Origin);
      encoder.EncodeKeyValuePair("Transform", instance.Transform);
      encoder.EncodeKeyValuePair("IsLocked", instance.IsLocked);
      encoder.EncodeKeyValuePair("SpriteEffects", instance.SpriteEffects);

      encoder.EncodeKeyValuePair("IsContinous", instance.IsContinous);
      encoder.EncodeKeyValuePair("CurrentFrame", instance.CurrentFrame);
      encoder.EncodeKeyValuePair("Frames", instance.Frames); 
      encoder.EncodeKeyValuePair("Target", instance.Target); 
    }
    public override void OnFoundCustomData(AnimatedSprite instance, string key, object value )
    {
    }
  }
}

