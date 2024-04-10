using ImGuiNET;
using Nez;
using Icon = IconFonts.FontAwesome5;

namespace Raven
{ 
	public class SheetInspector : Widget.PropertiedWindow
	{
    public override string Name { get => Sheet.Name; set => Sheet.Name = value;}
    public override PropertyList Properties { get => Sheet.Properties; set => Sheet.Properties = value; }

    public Sheet Sheet 
    { 
      get => _sheet; 
      set 
      { 
        if (value == null) return;
        if ((_sheet != null && _sheet.Name != value.Name) || _sheet == null)
        {
          _sheetData = new SheetPickerData(value); 
        }
        _sheet = value; 
      } 
    }
    Sheet _sheet;
    SpriteScene _spriteSceneOnOption;
    SheetPickerData _sheetData;

    public bool ShowPicker = false;
    public SpriteSceneSpritePicker SpritePicker;
    readonly internal EditorSettings _settings;

    public event Action<SpriteScene> OnClickScene;
    public event Action<SpriteScene> OnDeleteScene;

    public SheetInspector(EditorSettings settings, Camera camera)
    {
      _settings = settings;
      SpritePicker = new SpriteSceneSpritePicker(camera);
    }
    public override void Render(ImGuiWinManager imgui)
    {
      SpritePicker.EnableReselect = false;
      if (Sheet != null) base.Render(imgui);
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.ThLarge;
    }
    protected override void OnRenderAfterName()
    {
      int w = Sheet.TileWidth, h = Sheet.TileHeight;
      ImGui.LabelText(Icon.File + " File", Sheet.Source);
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

        var imageSize = ImGuiUtils.ContainSize(SpritePicker.SelectedSheet.Sheet.Size.ToNumerics(), new System.Numerics.Vector2(previewWidth, previewHeight));

        // Draws the selected spritesheet
        var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(SpritePicker.SelectedSheet.Sheet.Texture);
        ImGui.Image(texture, new System.Numerics.Vector2(imageSize.X, imageSize.Y));
        if (SpritePicker.OpenSheet == null && ImGui.IsItemHovered())
        {
          SpritePicker.OpenSheet = SpritePicker.SelectedSheet;
        }
        SpritePicker.Draw(new RectangleF(ImGui.GetItemRectMin().X, ImGui.GetItemRectMin().Y, 450, 450), _settings.Colors);
      }

      if (ImGui.CollapsingHeader($"{Icon.Th} Tiles ({Sheet.Tiles.X * Sheet.Tiles.Y})"))
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
      if (ImGui.CollapsingHeader($"{Icon.Users} SpriteScenes ({Sheet.SpriteScenees.Count})", ImGuiTreeNodeFlags.DefaultOpen))
      {
        ImGui.BeginChild("spriteScenees");
        ImGui.Indent();

        if (Sheet.SpriteScenees.Count == 0) ImGuiUtils.TextMiddle("No Scenes yet.");

        foreach (var spriteScene in Sheet.SpriteScenees)
        {
          if (ImGui.MenuItem($"{Icon.User} {spriteScene.Name}") && OnClickScene != null) 
          {
            OnClickScene(spriteScene);
          }
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
          {
            ImGui.OpenPopup("spriteScene-options-popup");
            _spriteSceneOnOption = spriteScene;
          }
        }
        ImGui.Unindent();
        ImGui.EndChild();
      }
      if (ImGui.BeginPopupContextItem("spriteScene-options-popup") && _spriteSceneOnOption != null)
      {
        if (ImGui.MenuItem(Icon.Pen + "  Rename"))
        {
          ImGuiManager.NameModal.Open((name)=>{Sheet.GetSpriteScene(_spriteSceneOnOption.Name).Name = name;});
        }
        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          Sheet.SpriteScenees.RemoveAll((spriteScene)=>spriteScene.Name == _spriteSceneOnOption.Name);
          if (Sheet.SpriteScenees.Count() == 0 && OnDeleteScene != null) 
            OnDeleteScene(_spriteSceneOnOption);
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate"))
        {
          Sheet.AddScene(_spriteSceneOnOption.Copy());
        }
        ImGui.EndPopup();
      }
      ImGui.End();
    }
	}
}
