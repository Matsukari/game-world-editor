
using Nez.Persistence;

namespace Raven.Serializers
{

  class TileJsonConverter : JsonTypeConverter<Tile>
  {
    public override bool WantsExclusiveWrite => true;
    public override bool CanWrite => true;     

    public override void WriteJson( IJsonEncoder encoder, Tile instance)
    {
      encoder.EncodeKeyValuePair("Name", instance.Name);
      encoder.EncodeKeyValuePair("Properties", instance.Properties);
      encoder.EncodeKeyValuePair("Coordinates", instance.Coordinates);
      encoder.EncodeKeyValuePair("SheetSource", instance._sheet.Name);
    }
    public override void OnFoundCustomData(Tile instance, string key, object value )
    {
      Console.WriteLine("OnFoundCustomData TIle: " + key);
      Console.WriteLine("value TIle: " + value);      
      JsonCache.Data["Tile"] = value;       
    }
  }
}

