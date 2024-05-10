
using Nez;
using ImGuiNET;

namespace Raven
{
  public class WorldInspector : Widget.PropertiedWindow
  {
    public override string Name { get => World.Name; set => World.Name = value;}
    public override PropertyList Properties { get => World.Properties; set => World.Properties = value; }
    readonly WorldViewImGui _viewImGui;

    World World { get => _viewImGui.Content as World; }
    SpritePicker _spritePicker { get => _viewImGui.SpritePicker; }
    List<LevelInspector> _levelInspectors { get => _viewImGui.LevelInspectors;}

    public WorldInspector(WorldViewImGui imgui) 
    {
      _viewImGui = imgui;
      OnAddSheet += (sheet) => World.AddSheet(sheet);
      OnAddSheet += (sheet) => 
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(new AddSheetCommand(World, sheet));

      OnRemoveSheet += sheet => 
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveSheetCommand(World, sheet));

      OnRemoveLevel += level => 
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveLevelCommand(World, level));

    }

    public event Action<Sheet> OnAddSheet;
    public event Action<Level> OnRemoveLevel;
    public event Action<Sheet> OnRemoveSheet;

    bool _isOpenSheetHeaderPopup = false;

    Widget.PopupDelegate _dialog = new Widget.PopupDelegate("dialog");
    public override void OutRender(ImGuiWinManager imgui)
    {
      if (_isOpenSheetHeaderPopup)
      {
        _isOpenSheetHeaderPopup = false;
        ImGui.OpenPopup("sheet-header-options-popup");
        Console.WriteLine("Opneed");
      }
      if (ImGui.BeginPopup("sheet-header-options-popup"))
      {
        if (ImGui.MenuItem("Add Sheet"))
        {
          void AddSheet(string file)
          {
            var content = Serializer.LoadContent<Sheet>(file);
            if (Serializer.SheetStdExtensions.Contains(Path.GetExtension(file)))
            {
              if (content == null)
              {
                _dialog.Open(im=>ImGuiUtils.TextMiddle("File is currupt."));
                return;
              }
              OnAddSheet(content);
            }
          }
          imgui.FilePicker.Open(AddSheet, "Open Sheet");
        }
        ImGui.EndPopup();
      }
      _dialog.Render(imgui);
    }    
    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Position", $"{World.Bounds.Location}");
      ImGui.LabelText("Width", $"{World.Bounds.Size.X} px");
      ImGui.LabelText("Height", $"{World.Bounds.Size.Y} px");
      ImGui.EndDisabled();

      // Another dock, levels panel
      var stack = new System.Numerics.Vector2(0, 0);
      var levelFlags = ImGuiTreeNodeFlags.None;
      if (World.Levels.Count() > 0) levelFlags = ImGuiTreeNodeFlags.DefaultOpen;



      // Levels listings
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.ObjectGroup + "   Levels", levelFlags))
      {
        var size = 200;
        stack.Y += size;
        ImGui.BeginChild("levels-content", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, size));
        for (int i = 0; i < _levelInspectors.Count(); i++)
        {
          var level = _levelInspectors[i];

          if (!level.Level.IsVisible)
            ImGui.PushStyleColor(ImGuiCol.Text, EditorColors.Get(ImGuiCol.Text));
          if (ImGui.Selectable($"{i+1}. {level.Name}", ref level.Selected, ImGuiSelectableFlags.AllowItemOverlap))
          {
            // Clear selection if not held with CTRL
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var levelGui in _levelInspectors)
              {
                levelGui.Selected = false;
              }
            }
            level.Selected = true;
          }
          ImGuiUtils.SpanX((ImGui.GetContentRegionAvail().X - ImGuiUtils.CalcTextSizeHorizontal(level.Name).X - 100));
          ImGui.PushID($"level-{level.Name}-id");
          var visibState = (!level.Level.IsVisible) ? IconFonts.FontAwesome5.EyeSlash : IconFonts.FontAwesome5.Eye;
          if (ImGui.SmallButton(visibState))
          {
            var previous = level.Level.IsVisible;
            level.Level.IsVisible = !level.Level.IsVisible;
            _viewImGui.SelectOtherLevel();
            Core.GetGlobalManager<CommandManagerHead>().Current.Record(new ModifyClassFieldCommand(level.Level, "IsVisible", previous));
          }
          ImGui.SameLine();
          if (ImGui.SmallButton(IconFonts.FontAwesome5.Times) && level.Level.IsVisible)
          {
            World.RemoveLevel(level.Level);
            if (OnRemoveLevel != null)
              OnRemoveLevel(level.Level);
          }
          if (!level.Level.IsVisible)
            ImGui.PopStyleColor();

          ImGui.PopID();
          stack.Y += ImGui.GetItemRectSize().Y;
        }
        ImGui.EndChild();
      }
      SheetPickerData removeSheet = null;

      // Spritesheet listings
      var node = ImGui.CollapsingHeader(IconFonts.FontAwesome5.BorderAll + "   SpriteSheets", ImGuiTreeNodeFlags.DefaultOpen);
      if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) _isOpenSheetHeaderPopup = true;
      if (node)
      {
        var size = 140;
        stack.Y += size;
        ImGui.BeginChild("spritesheets-content", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, size));
        for (int i = 0; i < _spritePicker.Sheets.Count(); i++)
        {
          var sheet = _spritePicker.Sheets[i];
          
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.NoTreePushOnOpen 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 
          if (sheet.Selected)
          {
            flags |= ImGuiTreeNodeFlags.Selected;
          }
          var sheetNode = ImGui.TreeNodeEx(sheet.Sheet.Name.BestWrap(20), flags); 

          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var sheetJ in _spritePicker.Sheets) sheetJ.Selected = false;
            }
            sheet.Selected = true;
          }

          ImGuiUtils.SpanX((ImGui.GetContentRegionMax().X - ImGuiUtils.CalcTextSizeHorizontal(sheet.Sheet.Name.BestWrap(20)).X - 100));
          if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
          {
            removeSheet = sheet;
          }

          if (sheetNode)
          {
            _spritePicker.SelectedSheet = sheet;
            // Draw preview spritesheet
            float previewHeight = 100;
            float previewWidth = ImGui.GetWindowWidth()-ImGui.GetStyle().WindowPadding.X*2-3; 

            var imageSize = ImGuiUtils.ContainSize(_spritePicker.SelectedSheet.Sheet.Size.ToNumerics(), new System.Numerics.Vector2(previewWidth, previewHeight));

            // Draws the selected spritesheet
            var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(_spritePicker.SelectedSheet.Sheet.Texture);
            ImGui.Image(texture, new System.Numerics.Vector2(imageSize.X, imageSize.Y));
            if (_spritePicker.OpenSheet == null && ImGui.IsItemHovered())
            {
              _spritePicker.OpenSheet = _spritePicker.SelectedSheet;
            }
          }
        }
        ImGui.EndChild();
      }
      if (removeSheet != null)
      {
        _spritePicker.Sheets.Remove(removeSheet);
        World.Sheets.Remove(removeSheet.Sheet);
        if (OnRemoveSheet != null)
          OnRemoveSheet(removeSheet.Sheet);
      }

      _spritePicker.Draw(new Nez.RectangleF(0f, Screen.Height-450-28, 450, 450), _viewImGui.Settings.Colors);
    }      
  }

}
