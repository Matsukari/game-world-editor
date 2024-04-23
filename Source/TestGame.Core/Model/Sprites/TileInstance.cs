using Nez.Persistence;

namespace Raven 
{
  public class TileInstance : IPropertied, ICloneable
  {
    [JsonInclude]
    public string Name { get; set; } = "";

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    public RenderProperties Props { get; private set; }

    [JsonInclude]
    public Tile Tile { get; private set; }

    internal TileInstance()
    {
    }
    
    public TileInstance(Tile tile, RenderProperties props=null)
    {
      Tile = tile;
      // if (props == null) props = new RenderProperties();
      Props = props;
    }

    object ICloneable.Clone()
    {
      var instance = MemberwiseClone() as TileInstance;
      instance.Properties = Properties.Copy();
      instance.Props = Props.Copy();
      return instance;

    }
  }

}
