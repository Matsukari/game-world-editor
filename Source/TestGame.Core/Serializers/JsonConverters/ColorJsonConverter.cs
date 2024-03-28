using Nez.Persistence;
using Microsoft.Xna.Framework;

namespace Raven.Serializers
{

  class ColorJsonConverter : JsonTypeConverter<Color>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Color color)
    {
      encoder.EncodeKeyValuePair("PackedValue", color.PackedValue);
    }
    public override void OnFoundCustomData(Color instance, string key, object value )
    {
    }
  }
}

