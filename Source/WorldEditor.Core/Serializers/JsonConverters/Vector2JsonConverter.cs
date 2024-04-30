using Nez.Persistence;
using Microsoft.Xna.Framework;

namespace Raven.Serializers
{

  class Vector2JsonConverter : JsonTypeConverter<Vector2>
  {
    public override bool WantsExclusiveWrite => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Vector2 color)
    {
      // encoder.EncodeKeyValuePair("", color.PackedValue);
    }
    public override void OnFoundCustomData(Vector2 instance, string key, object value )
    {
      // Console.WriteLine($"Found {key}: {value}");
      // if (key == "PackedValue") instance.PackedValue = (uint)value;
    }
  }
}

