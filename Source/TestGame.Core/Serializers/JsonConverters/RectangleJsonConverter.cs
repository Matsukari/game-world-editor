using Nez.Persistence;
using Microsoft.Xna.Framework;

namespace Raven.Serializers
{

  class RectangleJsonConverter : JsonTypeConverter<Rectangle>
  {
    public override bool WantsExclusiveWrite => true;
         
    public override void WriteJson( IJsonEncoder encoder, Rectangle color)
    {
      encoder.EncodeKeyValuePair("X", color.X);
      encoder.EncodeKeyValuePair("Y", color.Y);
      encoder.EncodeKeyValuePair("Width", color.Width);
      encoder.EncodeKeyValuePair("Height", color.Height);

    }
    public override void OnFoundCustomData(Rectangle instance, string key, object value )
    {
    }
  }
}

