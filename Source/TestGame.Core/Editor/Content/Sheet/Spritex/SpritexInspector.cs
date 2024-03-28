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
    public override PropertyList Properties { get => Spritex.Properties; set => Spritex.Properties = value; }
    
    public SourcedSprite ChangePart = null;
    SourcedSprite  _spritexPart;
    SpritexView _view;
    // Gui state data
    public Spritex Spritex;
    public Vector2 GuiPosition = new Vector2();
    public float GuiZoom = 0.5f;


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
    public static void RenderSprite(SourcedSprite sprite, bool drawName = true)
    {
      string name = sprite.Name;
      if (drawName && ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
      {
        sprite.Name = name;
      }
      ImGui.BeginDisabled();
        if (sprite.SourceSprite.Name != "") ImGui.LabelText("Source", sprite.SourceSprite.Name);
        ImGui.LabelText("Region", sprite.SourceSprite.Region.RenderStringFormat());
      ImGui.EndDisabled();

      sprite.Transform.RenderImGui();
      var origin = sprite.Origin.ToNumerics();

      var originType = sprite.DeterminePreset();
      // Preset origin
      if (ImGui.Combo("Origin", ref originType, _originTypes, _originTypes.Count()))
      {
        if (originType == 0) sprite.Origin = sprite.LocalBounds.Size/2f;
        else if (originType == 1) sprite.Origin = new Vector2();
      }
      // Custom origin is selected
      if (originType == 2)
      {
        if (ImGui.InputFloat2("Origin", ref origin)) sprite.Origin = origin;
      }
    }
    bool _isOpenComponentOptionPopup = false;
    SourcedSprite _compOnOptions = null;
    void DrawOptions()
    {
      if (_isOpenComponentOptionPopup)
      {
        _isOpenComponentOptionPopup = false;
        ImGui.OpenPopup("sprite-component-options");
      }
      if (ImGui.BeginPopupContextItem("sprite-component-options") && _compOnOptions != null)
      {
        var lockState = (_compOnOptions.IsLocked) ? IconFonts.FontAwesome5.LockOpen + "  Unlock" : IconFonts.FontAwesome5.Lock + "  Lock";
        if (ImGui.MenuItem(lockState))
        {
          _compOnOptions.IsLocked = !_compOnOptions.IsLocked;
        }
        var visib = (_compOnOptions.IsVisible) ? IconFonts.FontAwesome5.EyeSlash + "  Hide" : IconFonts.FontAwesome5.Eye + "  Show";
        if (ImGui.MenuItem(visib))
        {
          _compOnOptions.IsVisible = !_compOnOptions.IsVisible;
        }
        if (ImGui.MenuItem(IconFonts.FontAwesome5.Trash + "  Delete"))
        {
          _compOnOptions.DetachFromSpritex();
        }
        if (ImGui.MenuItem(IconFonts.FontAwesome5.Clone + "  Duplicate"))
        {
          ImGui.CloseCurrentPopup();
        }
        ImGui.EndPopup();
      }
    }
    void DrawComponentOptions(SourcedSprite sprite, ref SourcedSprite removeSprite)
    {
      // Options next to name
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(ImGui.GetWindowSize().X - ImGui.CalcTextSize(sprite.Name).X - 140, 0f));
      ImGui.SameLine();
      ImGui.PushID($"spritex-component-{sprite.Name}-options");
      var visibState = (!sprite.IsVisible) ? IconFonts.FontAwesome5.EyeSlash : IconFonts.FontAwesome5.Eye;
      if (ImGui.SmallButton(visibState))
      {
        sprite.IsVisible = !sprite.IsVisible;
      }
      ImGui.SameLine();
      var lockState = (!sprite.IsLocked) ? IconFonts.FontAwesome5.LockOpen: IconFonts.FontAwesome5.Lock;
      if (ImGui.SmallButton(lockState))
      {
        sprite.IsLocked = !sprite.IsLocked;
      }
      ImGui.SameLine();
      if (ImGui.SmallButton(IconFonts.FontAwesome5.Times))
      {
        removeSprite = sprite;
      }
      ImGui.PopID();

    }
    void DrawAnimationOptionPopup()
    {
      if (_isOpenAnimationOptionPopup)
      {
        _isOpenAnimationOptionPopup = false;
        ImGui.CloseCurrentPopup();
        ImGui.OpenPopup("animation-options-popup");
      }
      if (ImGui.BeginPopup("animation-options-popup"))
      {
        if (ImGui.MenuItem("Create Animation"))
        {
          Spritex.Animations.Add(new Animation(Spritex, "new-animation"));
        }
        ImGui.EndPopup();
      }
    }
    List<bool> _selectedSprites = new List<bool>();
    List<bool> _selectedAnims = new List<bool>();
    bool _isOpenAnimationOptionPopup = false;
    protected override void OnRenderAfterName()
    {
      _selectedSprites.EqualFalseRange(Spritex.Parts.Count());      
      _selectedAnims.EqualFalseRange(Spritex.Animations.Count());

      Transform.RenderImGui(Spritex.Transform);
      var animheader = ImGui.CollapsingHeader("Animations", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.AllowItemOverlap);
      if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
      {
        _isOpenAnimationOptionPopup = true;
      }
      if (animheader)
      {

        ImGui.BeginChild($"spritex-anim-child", new System.Numerics.Vector2(ImGui.GetWindowWidth(), 200), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);
        for (int i = 0; i < Spritex.Animations.Count(); i++)
        {
          var animation = Spritex.Animations[i];
          var isSelected = _selectedAnims[i];
          if (ImGui.Selectable($"{i+1}. {animation.Name}", ref isSelected, ImGuiSelectableFlags.AllowItemOverlap))
          {
            if (!ImGui.GetIO().KeyCtrl) _selectedAnims.FalseRange(_selectedAnims.Count());
            _selectedAnims[i] = true;
            _view.Editor.GetEditorComponent<AnimationEditor>().Open(Spritex, animation);
          }
        }

        ImGui.EndChild();
      }
      if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding))
      {
        ImGui.BeginChild($"spritex-comp-content-child", new System.Numerics.Vector2(ImGui.GetWindowWidth(), 200), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

        SourcedSprite removeSprite = null;
        for (int i = 0; i < _selectedSprites.Count; i++)
        {
          var part = Spritex.Parts[i];
          var isSelected = _selectedSprites[i];
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.NoTreePushOnOpen 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 
          if (isSelected) flags |= ImGuiTreeNodeFlags.Selected;
          
          var spriteNode = ImGui.TreeNodeEx(part.Name, flags);

          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              for (int j = 0; j < _selectedSprites.Count; j++) _selectedSprites[j] = false;
            }
            _selectedSprites[i] = true;
          }

          DrawComponentOptions(part, ref removeSprite);

          if (spriteNode)
          {
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
              _isOpenComponentOptionPopup = true;
              _compOnOptions = part;
            }
            ImGui.PushID("spritex-component-content-" + part.Name);
            RenderSprite(part);
            ImGui.PopID();

            ImGui.TreePop();
          }
          if (part.WorldBounds.Contains(_view.Entity.Scene.Camera.MouseToWorldPoint()) && Nez.Input.RightMouseButtonPressed)
          {
            _isOpenComponentOptionPopup = true;
            _compOnOptions = part;
          }
        }
        ImGui.EndChild();
        DrawOptions();
        DrawAnimationOptionPopup();
        if (removeSprite != null) removeSprite.DetachFromSpritex();
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
