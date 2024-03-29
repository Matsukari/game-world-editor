
using Nez.Persistence;
using Raven.Sheet.Sprites;

namespace Raven.Serializers
{

  class SourcedSpriteJsonConverter : JsonTypeConverter<SourcedSprite>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, SourcedSprite instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("IsVisible", instance.IsVisible);
      encoder.EncodeKeyValuePair("Color", instance.Color);
      encoder.EncodeKeyValuePair("Origin", instance.Origin);
      encoder.EncodeKeyValuePair("Transform", instance.Transform);
      encoder.EncodeKeyValuePair("IsLocked", instance.IsLocked);
      encoder.EncodeKeyValuePair("SpriteEffects", instance.SpriteEffects);
      encoder.EncodeKeyValuePair("SourceSprite", instance.SourceSprite);

    }
    public override void OnFoundCustomData(SourcedSprite instance, string key, object value )
    {
    }
  }
}

