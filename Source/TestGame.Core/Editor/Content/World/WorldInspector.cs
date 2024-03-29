
using Nez;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public class WorldInspector : Widget.PropertiedWindow
  {
    public override string Name { get => World.Name; set => World.Name = value;}
    public override PropertyList Properties { get => World.Properties; set => World.Properties = value; }

    Editor _editor;
    public World World;
    public WorldEditor WorldEditor;
    bool _isOpenSheetHeaderPopup = false;
    public override void Render(Editor editor)
    {
      _editor = editor;
      if (WorldEditor != null)
      {
        World = WorldEditor._world;
        base.Render(editor);
      }

      if (_isOpenSheetHeaderPopup)
      {
        _isOpenSheetHeaderPopup = false;
        ImGui.OpenPopup("sheet-header-options-popup");
      }
      if (ImGui.BeginPopupContextItem("sheet-header-options-popup"))
      {
        if (ImGui.MenuItem("Add Sheet"))
        {
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


      if (WorldEditor._spritePicker.Sheets.Count() != WorldEditor._world.SpriteSheets.Count())
      {
        WorldEditor._spritePicker.Sheets.Clear();
        foreach (var sheet in WorldEditor._world.SpriteSheets) 
        {
          var data = new SheetPickerData(sheet.Value, _editor.Settings.Colors);
          WorldEditor._spritePicker.Sheets.Add(data);
        }
      }
      // Levels listings
      if (ImGui.CollapsingHeader(IconFonts.FontAwesome5.ObjectGroup + "   Levels", levelFlags))
      {
        var size = 200;
        stack.Y += size;
        ImGui.BeginChild("levels-content", new System.Numerics.Vector2(ImGui.GetWindowWidth(), size));
        for (int i = 0; i < WorldEditor._levelInspectors.Count(); i++)
        {
          var level = WorldEditor._levelInspectors[i];
          if (ImGui.Selectable($"{i+1}. {level.Name}", ref level.Selected, ImGuiSelectableFlags.AllowItemOverlap))
          {
            // Clear selection if not held with CTRL
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var levelGui in WorldEditor._levelInspectors)
              {
                levelGui.Selected = false;
              }
            }
            WorldEditor.SelectedLevel = i;
            level.Selected = true;
          }
          ImGui.SameLine();
          ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(level.Name).X - 100, 0f));
          ImGui.SameLine();
          ImGui.PushID($"level-{level.Name}-id");
          var visibState = (!level._level.Enabled) ? IconFonts.FontAwesome5.EyeSlash : IconFonts.FontAwesome5.Eye;
          if (ImGui.SmallButton(visibState))
          {
            level._level.Enabled = !level._level.Enabled;
          }
          ImGui.SameLine();
          if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
          {
            World.RemoveLevel(level._level);
            WorldEditor.Editor.GetEditorComponent<Selection>().End();
            WorldEditor.SelectedLevel = -1;
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
        for (int i = 0; i < WorldEditor._spritePicker.Sheets.Count(); i++)
        {
          var sheet = WorldEditor._spritePicker.Sheets[i];
          
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.NoTreePushOnOpen 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 
          if (sheet.Selected)
          {
            flags |= ImGuiTreeNodeFlags.Selected;
          }
          var name = sheet.Sheet.Name;
          if (name.Count() > 20) name = name.Substring(0, 20) + "...";
          var sheetNode = ImGui.TreeNodeEx(sheet.Sheet.Name, flags); 

          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              foreach (var sheetJ in WorldEditor._spritePicker.Sheets) sheetJ.Selected = false;
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
            WorldEditor._spritePicker.SelectedSheet = sheet;
            // Draw preview spritesheet
            float previewHeight = 100;
            float previewWidth = ImGui.GetWindowWidth()-ImGui.GetStyle().WindowPadding.X*2-3; 
            float ratio = (previewWidth) / previewHeight;

            // Draws the selected spritesheet
            var texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(WorldEditor._spritePicker.SelectedSheet.Sheet.Texture);
            ImGui.Image(texture, new System.Numerics.Vector2(previewWidth, previewHeight*ratio), 
                WorldEditor._spritePicker.GetUvMin(WorldEditor._spritePicker.SelectedSheet), 
                WorldEditor._spritePicker.GetUvMax(WorldEditor._spritePicker.SelectedSheet));
            if (WorldEditor._spritePicker.OpenSheet == null && ImGui.IsItemHovered())
            {
              WorldEditor._spritePicker.OpenSheet = WorldEditor._spritePicker.SelectedSheet;
            }
          }
        }
        ImGui.EndChild();
      }
      if (removeSheet != null)
      {
        WorldEditor._spritePicker.Sheets.Remove(removeSheet);
        World.SpriteSheets.Remove(removeSheet.Sheet.Name);
      }
    }      
  }

}
