using ImGuiNET;
using Raven.Sheet.Sprites;

namespace Raven
{
  public class SpriteInspector: Widget.PropertiedWindow
  {
    public override string Name { get => Sprite.Name; set => Sprite.Name = value;}
    public override PropertyList Properties { get => Sprite.Properties; set => Sprite.Properties = value; }

    public Sprite Sprite;
    public override void Render(Editor editor)
    {
      if (Sprite != null) base.Render(editor);
    }    
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.GripHorizontal;
    } 
    protected override void OnChangeProperty(string name)
    {
      foreach (var tile in Sprite._tiles)
      {
         var existingTile = Sprite._sheet.GetTileData(tile);
        // If not created yet
        Sprite._sheet.CreateTile(existingTile);
        existingTile.Properties.OverrideOrAddAll(Properties);
      }
    }
    protected override void OnChangeName(string prev, string curr)
    {
      foreach (var tile in Sprite._tiles)
      {
        // If not created yet
        var existingTile = Sprite._sheet.GetTileData(tile);
        Sprite._sheet.CreateTile(existingTile);
        existingTile.Name = curr;
      }
    } 
    protected override void OnRenderAfterName()
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Tiles", Sprite._tiles.Count.ToString());
      ImGui.LabelText("Region", Sprite.Region.RenderStringFormat());
      ImGui.EndDisabled();
    }

  }
}
