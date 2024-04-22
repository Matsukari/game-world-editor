

using Nez.Persistence;

namespace Raven.Serializers
{

  class EditorContentDataJsonConverter : JsonTypeConverter<EditorContentData>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, EditorContentData instance)
    {
      encoder.EncodeKeyValuePair("Filename", instance.Filename);
      encoder.EncodeKeyValuePair("Type", instance.Type);
      encoder.EncodeKeyValuePair("Position", instance.Position);
      encoder.EncodeKeyValuePair("Zoom", instance.Zoom);
    }
    public override void OnFoundCustomData(EditorContentData instance, string key, object value )
    {
    }
  }
}

