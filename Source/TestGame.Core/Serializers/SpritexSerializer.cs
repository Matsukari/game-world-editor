using Raven.Sheet.Sprites;
using Nez.Persistence;

namespace Raven.Serializers
{

  class SpritexSerializer : JsonTypeConverter<Spritex>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Spritex spritex)
    {
      encoder.EncodeKeyValuePair("Name", spritex.Name);
      encoder.EncodeKeyValuePair("Properties", spritex.Properties);
      encoder.EncodeKeyValuePair("Animations", spritex.Animations);
      encoder.EncodeKeyValuePair("Parts", spritex.Parts);
      // encoder.EncodeKeyValuePair("Color", spritex.Color);
    }

    public override void OnFoundCustomData(Spritex instance, string key, object value )
    {
      Console.WriteLine($"Fuck {key}: {value}");
    }
  }
}

