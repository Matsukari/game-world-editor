using ImGuiNET;

namespace Raven
{
  public class TileInspector : Widget.PropertiedWindow
  { 
    public override string Name { get => Tile.Name; set => Tile.Name = value;}
    public override PropertyList Properties { get => Tile.Properties; set => Tile.Properties = value; }
    public override bool CanOpen => Tile != null; 

    public Tile Tile;
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.BorderNone;
    }
    protected override void OnChangeName(string prev, string curr)
    {
      if (Tile._sheet.CreateTile(Tile)) Console.WriteLine("Created tile");
      Tile._sheet.GetCreatedTile(Tile.Id).Name = curr;
    }        
    protected override void OnChangeProperty(string name)
    {
      if (Tile._sheet.CreateTile(Tile)) Console.WriteLine("Created tile");
    }
    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Id", Tile.Id.ToString());
      ImGui.LabelText("Tile", $"{Tile.Coordinates.X}x, {Tile.Coordinates.Y}y");
      ImGui.EndDisabled();
    }
  }   
}