
using Nez.Persistence;
using ImGuiNET;

namespace Raven.Serializers
{

  class EditorSettingsJsonConverter : JsonTypeConverter<EditorSettings>
  {
    public override bool WantsExclusiveWrite => true;
    public override bool CanWrite => true;     

    public override void WriteJson( IJsonEncoder encoder, EditorSettings instance)
    {
      // encoder.EncodeKeyValuePair("ImGuiColors", ImGui.GetStyle().colors)
    }
    public override void OnFoundCustomData(EditorSettings instance, string key, object value )
    {
    }
  }
}

