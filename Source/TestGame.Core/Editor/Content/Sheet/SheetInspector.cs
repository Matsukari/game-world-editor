using ImGuiNET;
using Nez;

namespace Raven.Sheet
{ 
	public class SheetInspector : Widget.PropertiedWindow
	{
    public override string Name { get => Sheet.Name; set => Sheet.Name = value;}
    public override PropertyList Properties { get => Sheet.Properties; set => Sheet.Properties = value; }

    public Sheet Sheet { 
      get => _sheet; 
      set { _sheet = value; _sheetData = new SheetPickerData(_sheet, _editor.Settings.Colors); } 
    }
    Sheet _sheet;
    internal Editor _editor;
    Sprites.Spritex _spritexOnOption;
    public bool ShowPicker = false;
    public SpritePicker SpritePicker = new SpritePicker();
    SheetPickerData _sheetData;

    public override void Render(Editor editor)
    {
      _editor = editor;
      SpritePicker.EnableReselect = false;
      if (Sheet != null) base.Render(editor);
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    protected override void OnRenderAfterName()
    {
      int w = Sheet.TileWidth, h = Sheet.TileHeight;
      ImGui.LabelText(IconFonts.FontAwesome5.File + " File", Sheet.Source);
      if (ImGui.InputInt("TileWidth", ref w)) Sheet.SetTileSize(w, Sheet.TileHeight);
      if (ImGui.InputInt("TileHeight", ref h)) Sheet.SetTileSize(Sheet.TileWidth, h);
      ImGui.BeginDisabled();
      ImGui.LabelText("Width", $"{Sheet.Tiles.X} tiles");
      ImGui.LabelText("Height", $"{Sheet.Tiles.Y} tiles");
      ImGui.EndDisabled();

      if (ShowPicker && ImGui.CollapsingHeader("Preview", ImGuiTreeNodeFlags.DefaultOpen))
      {
        SpritePicker.SelectedSheet = _sheetData;
        // Draw preview spritesheet
        float previewHeight = 100;
        float previewWidth = ImGui.GetWindowWidth()-ImGui.GetStyle().WindowPadding.X*2-3; 
        float ratio = (previewWidth) / previewHeight;

        // Draws the selected spritesheet
        var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(SpritePicker.SelectedSheet.Sheet.Texture);
        ImGui.Image(texture, new System.Numerics.Vector2(previewWidth, previewHeight*ratio), 
            SpritePicker.GetUvMin(SpritePicker.SelectedSheet), 
            SpritePicker.GetUvMax(SpritePicker.SelectedSheet));
        if (SpritePicker.OpenSheet == null && ImGui.IsItemHovered())
        {
          SpritePicker.OpenSheet = SpritePicker.SelectedSheet;
        }
        SpritePicker.Draw(new RectangleF(ImGui.GetItemRectMin().X, ImGui.GetItemRectMin().Y, 450, 450));
      }

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
