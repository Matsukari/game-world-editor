using ImGuiNET;
using Nez;

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
      if (Tile._sheet.CreateTile(Tile)) 
      {
        Console.WriteLine("Created tile");
        AddTile();
      }
      Tile._sheet.GetCreatedTile(Tile.Id).Name = curr;
    }        
    protected override void OnChangeProperty(string name)
    {
      if (Tile._sheet.CreateTile(Tile)) 
      {
        Console.WriteLine("Created tile");
        AddTile();
      }
    }
    void AddTile()
    {
      Core.GetGlobalManager<CommandManagerHead>().Current.MergeCurrent(new AddTileCommand(Tile._sheet, Tile));
    }

    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Id", Tile.Id.ToString());
      ImGui.LabelText("Tile", $"{Tile.Coordinates.X}x, {Tile.Coordinates.Y}y");
      ImGui.EndDisabled();

      ImGuiUtils.DrawImage(Tile.Sprite, new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X));
    }
  }   
}
