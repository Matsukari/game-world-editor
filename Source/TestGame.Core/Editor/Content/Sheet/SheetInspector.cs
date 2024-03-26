using ImGuiNET;

namespace Raven.Sheet
{ 
	public class SheetInspector : Widget.PropertiedWindow
	{
    public override string Name { get => Sheet.Name; set => Sheet.Name = value;}
    public Sheet Sheet;
    Editor _editor;
    Sprites.Spritex _spritexOnOption;

    public override void Render(Editor editor)
    {
      _editor = editor;
      if (Sheet != null) base.Render(editor);
    }
        
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    protected override void OnRenderAfterName()
    {
      int w = Sheet.TileWidth, h = Sheet.TileHeight;
      ImGui.LabelText(IconFonts.FontAwesome5.File + " File", Sheet.Texture.Name);
      if (ImGui.InputInt("TileWidth", ref w)) Sheet.SetTileSize(w, Sheet.TileHeight);
      if (ImGui.InputInt("TileHeight", ref h)) Sheet.SetTileSize(Sheet.TileWidth, h);
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{Sheet.Tiles.X} tiles");
      ImGui.LabelText("Height", $"{Sheet.Tiles.Y} tiles");
      ImGui.EndDisabled();

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
        foreach (var spritex in Sheet.Spritexes)
        {
          if (ImGui.MenuItem($"{IconFonts.FontAwesome5.User} {spritex.Name}")) 
          {
            var spritexView = _editor.GetEditorComponent<SpritexView>();
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
          _editor.NameModal.Open((name)=>{Sheet.GetSpritex(_spritexOnOption.Name).Name = name;});
        }
        if (ImGui.MenuItem("Delete"))
        {
          Sheet.Spritexes.RemoveAll((spritex)=>spritex.Name == _spritexOnOption.Name);
          if (Sheet.Spritexes.Count() == 0) _editor.GetEditorComponent<SpritexView>().UnEdit();
        }
        ImGui.EndPopup();
      }
      ImGui.End();
    }
	}
}
