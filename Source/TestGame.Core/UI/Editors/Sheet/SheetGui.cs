using ImGuiNET;

namespace Raven.Sheet
{ 
	public class SheetGui : Propertied
	{
    public override string Name { get => _sheet.Name; set => _sheet.Name = value;}
        
    internal Sheet _sheet;
    public SheetGui(Sheet sheet)
    {
      _sheet = sheet;
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      int w = _sheet.TileWidth, h = _sheet.TileHeight;
      ImGui.LabelText(IconFonts.FontAwesome5.File + " File", _sheet.Texture.Name);
      if (ImGui.InputInt("TileWidth", ref w)) _sheet.SetTileSize(w, _sheet.TileHeight);
      if (ImGui.InputInt("TileHeight", ref h)) _sheet.SetTileSize(_sheet.TileWidth, h);
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{_sheet.Tiles.X} tiles");
      ImGui.LabelText("Height", $"{_sheet.Tiles.Y} tiles");
      ImGui.EndDisabled();
    }
	}
}
