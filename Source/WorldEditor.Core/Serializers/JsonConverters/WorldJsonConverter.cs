
using Nez.Persistence;

namespace Raven.Serializers
{

  class WorldJsonConverter : JsonTypeConverter<World>
  {
    public override bool WantsExclusiveWrite => true;
    public override bool CanWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, World instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Levels", instance.Levels);
      encoder.EncodeKeyValuePair("Position", instance.Position);

      List<string> sources = new List<string>();
      foreach (var sheet in instance.Sheets)
      {
        sources.Add(sheet.Name);
      }
      encoder.EncodeKeyValuePair("SheetSources", sources);
    }
    public override void OnFoundCustomData(World instance, string key, object value )
    {
      Console.WriteLine("OnFoundCustomData World: " + key);
      Console.WriteLine("value World: " + value);      
      JsonCache.Data.Add(key, value);
       
    }
  }
}

