using ImGuiNET;

namespace Raven
{
  public class TileInstanceInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Tile.Name; set => Tile.Name = value;}
    public override PropertyList Properties { get => Tile.Properties; set => Tile.Properties = value; }
    public override bool CanOpen => Tile != null && Layer != null;
        
    public TileInstance Tile;
    public TileLayer Layer;
    public event Action<TileInstance, TileLayer> OnTileModified;

    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Id", Tile.Tile.Id.ToString());
      ImGui.LabelText("Tile", $"{Tile.Tile.Coordinates.X}x, {Tile.Tile.Coordinates.Y}y");
      ImGui.EndDisabled();
      if (Tile.Props == null)
      {
        if (ImGui.Button(IconFonts.FontAwesome5.Plus + "   Add Custom Render Attributes"))
          Tile.Props = new RenderProperties();
      }
      else if (Tile.Props.Transform.RenderImGui() && OnTileModified != null) OnTileModified(Tile, Layer);
      
    } 
  }
}

