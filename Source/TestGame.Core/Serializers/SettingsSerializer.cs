
namespace Raven.Serializers
{
  class SettingsSerializer : JsonSerializer<EditorSettings>
  {
    protected override EditorSettings Realize(EditorSettings obj)
    {
      return obj;
    }    
  }
}
