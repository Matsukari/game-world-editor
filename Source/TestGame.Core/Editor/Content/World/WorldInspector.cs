
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

    public WorldInspector(WorldViewImGui imgui) => _viewImGui = imgui;

    public event Action<string> OnAddSheet;
    public event Action<Level> OnRemoveLevel;

    bool _isOpenSheetHeaderPopup = false;
    public override void Render(ImGuiWinManager imgui)
    {
      base.Render(imgui);

      if (_isOpenSheetHeaderPopup)
      {
        _isOpenSheetHeaderPopup = false;
        ImGui.OpenPopup("sheet-header-options-popup");
      }
      if (ImGui.BeginPopupContextItem("sheet-header-options-popup"))
      {
        if (ImGui.MenuItem("Add Sheet"))
        {
          void AddSheet(string file)
          {
            if (Serializer.SheetStdExtensions.Contains(Path.GetExtension(file)) && OnAddSheet != null)
            {
              OnAddSheet(file);
            }
          }
          imgui.FilePicker.Open(AddSheet);
        }
        ImGui.EndPopup();
      }
    }    
    protected override void OnRenderAfterName()
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
        ImGui.BeginChild("levels-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        for (int i = 0; i < _levelInspectors.Count(); i++)
        {
          var level = _levelInspectors[i];
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
          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(level.Name).X - 100, 0f));
          ImGui.SameLine();
          ImGui.PushID($"level-{level.Name}-id");
          var visibState = (!level.Level.IsVisible) ? IconFonts.FontAwesome5.EyeSlash : IconFonts.FontAwesome5.Eye;
          if (ImGui.SmallButton(visibState))
          {
            level.Level.IsVisible = !level.Level.IsVisible;
          }
          ImGui.SameLine();
          if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
          {
            World.RemoveLevel(level.Level);
            OnRemoveLevel(level.Level);
          }
          ImGui.PopID();
          stack.Y += ImGui.GetItemRectSize().Y;
        }
        ImGui.EndChild();
      }
      SheetPickerData removeSheet = null;

      // Spritesheet listings
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.BorderAll + "   SpriteSheets", ImGuiTreeNodeFlags.DefaultOpen))
      {
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.IsWindowHovered()) _isOpenSheetHeaderPopup = true;
        var size = 140;
        stack.Y += size;
        ImGui.BeginChild("spritesheets-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        for (int i = 0; i < _spritePicker.Sheets.Count(); i++)
        {
          var sheet = _spritePicker.Sheets[i];
          
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.NoTreePushOnOpen 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 
          if (sheet.Selected)
          {
            flags |= ImGuiTreeNodeFlags.Selected;
          }
          var sheetNode = ImGui.TreeNodeEx(sheet.Sheet.Name.BestWrap(), flags); 

          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var sheetJ in _spritePicker.Sheets) sheetJ.Selected = false;
            }
            sheet.Selected = true;
          }

          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(sheet.Sheet.Name).X - 100 , 0f));
          ImGui.SameLine();
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
      }

      _spritePicker.Draw(new Nez.RectangleF(0f, Screen.Height-450-28, 450, 450), _viewImGui.Settings.Colors);
    }      
  }

}
