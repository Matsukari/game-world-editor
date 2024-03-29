using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Persistence;

namespace Raven.Sheet.Sprites 
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

    public string Name = "";

    public Point Coordinates;

    public Rectangle Region { get=>_sheet.GetTile(Id); }

    public int Id { get=>_sheet.GetTileId(Coordinates.X, Coordinates.Y); }

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
  }   
}
