
using Nez.Persistence;
using Raven.Sheet.Sprites;

namespace Raven.Serializers
{

  class SpriteSceneJsonConverter : JsonTypeConverter<SpriteScene>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, SpriteScene instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Color", instance.Color);
      encoder.EncodeKeyValuePair("RenderLayer", instance.RenderLayer);
      encoder.EncodeKeyValuePair("Parts", instance.Parts);
      encoder.EncodeKeyValuePair("Animations", instance.Animations);
      encoder.EncodeKeyValuePair("IsVisible", instance.IsVisible);
      encoder.EncodeKeyValuePair("LocalOffset", instance.LocalOffset);
      encoder.EncodeKeyValuePair("UpdateOrder", instance.UpdateOrder);
    }
    public override void OnFoundCustomData(SpriteScene instance, string key, object value )
    {
    }
  }
}

