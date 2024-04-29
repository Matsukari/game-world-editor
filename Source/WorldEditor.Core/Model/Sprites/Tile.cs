using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Persistence;

namespace Raven 
{
  /// <summary>
  /// Simply an ID handle for a specific location in the Sheet's tilemap. 
  /// The Editor will take care of the instancing of the actual Tile in tilemap so manually 
  /// create a Tile when you add a name or property
  /// </summary>
  public class Tile : IPropertied
  { 
    string IPropertied.Name { get => Name; set => Name = value; }

    [JsonInclude]
    public PropertyList Properties { get; set; } = new PropertyList();

    /// <summary>
    /// Optional name to tag Tile. 
    /// </summary>
    public string Name = "";

    /// <summary>
    /// The position in the Sheet 
    /// </summary>
    public Point Coordinates;

    /// <summary>
    /// The rect region that contains the source for this Tile
    /// </summary>
    public Rectangle Region { get=>_sheet.GetTile(Id); }

    /// <summary>
    /// ID used to query Tile from sheet
    /// </summary>
    public int Id { get=>_sheet.GetTileId(Coordinates.X, Coordinates.Y); }

    /// <summary>
    /// Shortcut to the texture this Tile is bounds
    /// </summary>
    public Texture2D Texture { get => _sheet.Texture; }

    internal Sheet _sheet;

    private Tile() 
    {
    }

    public Tile(Point coord, Sheet sheet)
    {
      Coordinates = coord;
      _sheet = sheet;
      Insist.IsTrue(_sheet.IsTileValid(_sheet.GetTileId(coord.X, coord.Y)));
    }
    public override bool Equals(object obj)
    {
      if (obj is Tile tile && tile.Name == Name) 
        return tile.Id == Id;
      return false;
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    public Tile Copy() 
    {
      var tile = MemberwiseClone() as Tile;
      tile.Properties = Properties.Copy();
      return tile;
    }
  }   
}
