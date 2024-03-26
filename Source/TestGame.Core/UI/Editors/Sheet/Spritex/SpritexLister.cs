using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;


namespace Raven.Sheet
{
  public class SpritexLister : Editor.SheetEntity
  {
    public override void OnAddedToScene() => Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(RenderImGui);
    Sprites.Spritex _spritexOnOption;
    public void RenderImGui() 
    {
      if (!Enabled) return;
      ImGui.Begin(IconFonts.FontAwesome5.List + " " + GetType().Name);
      if (ImGui.CollapsingHeader($"{IconFonts.FontAwesome5.Th} Tiles ({Sheet.Tiles.X * Sheet.Tiles.Y})"))
      {
        foreach (var (name, tile) in Sheet.TileMap)
        {
          ImGui.Indent();
          if (ImGui.MenuItem($"{name}"))
          {
          }
          ImGui.Unindent();
        }
      }
      if (ImGui.CollapsingHeader($"{IconFonts.FontAwesome5.Users} Spritexes ({Sheet.Spritexes.Count})", ImGuiTreeNodeFlags.DefaultOpen))
      {
        ImGui.BeginChild("spritexes");
        ImGui.Indent();
        foreach (var (name, spritex) in Sheet.Spritexes)
        {
          if (ImGui.MenuItem($"{IconFonts.FontAwesome5.User} {name}")) 
          {
            var spritexView = Editor.GetSubEntity<SpritexView>();
            spritexView.Edit(spritex);
          }
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
          {
            ImGui.OpenPopup("spritex-options-popup");
            _spritexOnOption = spritex;
          }
        }
        ImGui.Unindent();
        ImGui.EndChild();
      }
      if (ImGui.BeginPopupContextItem("spritex-options-popup"))
      {
        if (ImGui.MenuItem("Rename"))
        {
          Editor.OpenNameModal((name)=>{Sheet.Spritexes.ChangeKey(_spritexOnOption.Name, name); _spritexOnOption.Name = name;});
        }
        if (ImGui.MenuItem("Delete"))
        {
          Sheet.Spritexes.Remove(_spritexOnOption.Name);
          if (Sheet.Spritexes.Count() == 0) Editor.GetSubEntity<SpritexView>().UnEdit();
        }
        ImGui.EndPopup();
      }
      ImGui.End();
    }
  }
}

