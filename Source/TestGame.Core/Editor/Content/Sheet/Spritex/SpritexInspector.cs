using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven.Sheet.Sprites 
{
  // <summary>
  // ImGui window for editing spritex properties
  // </summary>
  public class SpritexInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Spritex.Name; set => Spritex.Name = value; }
    
    public SourcedSprite ChangePart = null;
    SourcedSprite  _spritexPart;
    SpritexView _view;
    // Gui state data
    public Spritex Spritex;
    public Vector2 GuiPosition = new Vector2();
    public float GuiZoom = 0.5f;


    int _originType = 1;
    static string[] _originTypes = new string[] { "Center", "Topleft", "Custom" };

    public SpritexInspector(SpritexView view, Spritex spritex) 
    {
      _view = view;
      Spritex = spritex;
    }
    public override void Render(Editor editor)
    {
      if (Spritex != null && Spritex.Enabled) base.Render(editor);
    }
    void RenderSprite(SourcedSprite sprite)
    {
      ImGui.BeginDisabled();
        if (sprite.SourceSprite.Name != "") ImGui.LabelText("Source", sprite.SourceSprite.Name);
        ImGui.LabelText("Region", sprite.SourceSprite.Region.RenderStringFormat());
      ImGui.EndDisabled();

      sprite.Transform.RenderImGui();
      var origin = sprite.Origin.ToNumerics();
    
      // Preset origin
      if (ImGui.Combo("Origin", ref _originType, _originTypes, _originTypes.Count()))
      {
             if (_originType == 0) sprite.Origin = sprite.LocalBounds.Size/2f;
        else if (_originType == 1) sprite.Origin = new Vector2();
      }
      // Custom origin is selected
      if (_originType == 2)
      {
        if (ImGui.InputFloat2("Origin", ref origin)) sprite.Origin = origin;
      }
    }

    protected override void OnRenderAfterName()
    {
      Transform.RenderImGui(Spritex.Transform);
      if (ImGui.CollapsingHeader("Animations", ImGuiTreeNodeFlags.DefaultOpen))
      {
      }
      if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding))
      {
        foreach (var part in Spritex.Body)
        {
          if (ImGui.TreeNodeEx(part.Name, ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.DefaultOpen))
          {
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
              ImGui.TreePop();
              ImGui.OpenPopup("sprite-component-options");
              _spritexPart = part;
              return;
            }
            ImGui.PushID("spritex-component-name" + part.Name);
            string name = part.Name;
            if (ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
            {
              Spritex.Parts.Data.ChangeKey(part.Name, name);
              part.Name = name;
            }
            ImGui.PopID();
            RenderSprite(part);

            // Options
            ImGui.TreePop();
          }
          if (part.WorldBounds.Contains(_view.Entity.Scene.Camera.MouseToWorldPoint()) && Nez.Input.RightMouseButtonPressed)
          {
            ImGui.OpenPopup("sprite-component-options");
            _spritexPart = part;
          }
        }
        if (ImGui.BeginPopupContextItem("sprite-component-options"))
        {
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Trash + "  Delete"))
          {
            Console.WriteLine("Deleteing " + _spritexPart.Name);
            Spritex.Parts.Data.Remove(_spritexPart.Name);
            ImGui.CloseCurrentPopup();
          }
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Edit + "  Change region"))
          {
            ChangePart = _spritexPart;
            _view.UnEdit();
            ImGui.CloseCurrentPopup();
          }
          if (ImGui.MenuItem(IconFonts.FontAwesome5.Clone + "  Duplicate"))
          {
            ImGui.CloseCurrentPopup();
          }
          ImGui.EndPopup();
        }
      }
    }
    public override string GetIcon()
    {
      return IconFonts.FontAwesome5.User;
    }
    protected override void OnChangeName(string old, string now)
    {
    } 
  }
}
