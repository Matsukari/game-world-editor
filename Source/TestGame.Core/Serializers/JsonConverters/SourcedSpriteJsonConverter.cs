
using Nez.Persistence;

namespace Raven.Serializers
{

  class SceneSpriteJsonConverter : JsonTypeConverter<ISceneSprite>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, ISceneSprite instance)
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
    public override void OnFoundCustomData(ISceneSprite instance, string key, object value )
    {
    }
  }
}

