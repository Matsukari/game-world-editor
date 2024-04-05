using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;

namespace Raven 
{
  // <summary>
  // ImGui window for editing spriteScene properties
  // </summary>
  public class SpriteSceneInspector : Widget.PropertiedWindow
  {
    public override string Name { get => SpriteScene.Name; set => SpriteScene.Name = value; }
    public override PropertyList Properties { get => SpriteScene.Properties; set => SpriteScene.Properties = value; }
    
    public SourcedSprite ChangePart = null;
    SourcedSprite  _spriteScenePart;
    // Gui state data
    public SpriteScene SpriteScene;
    public Vector2 GuiPosition = new Vector2();
    public float GuiZoom = 0.5f;

    public event Action<SpriteScene, Animation> OnOpenAnimation;
    public event Action<SourcedSprite> OnAddPart;
    public event Action<SourcedSprite> OnDelPart;

    static string[] _originTypes = new string[] { "Center", "Topleft", "Custom" };

    public SpriteSceneInspector(SpriteScene spriteScene) 
    {
      SpriteScene = spriteScene;
    }
    public override void Render(ImGuiWinManager imgui)
    {
      if (SpriteScene != null) base.Render(imgui);
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
        if (originType == 0) sprite.Origin = sprite.Bounds.Size/2f;
        else if (originType == 1) sprite.Origin = new Vector2();
      }
      // Custom origin is selected
      if (originType == 2)
      {
        if (ImGui.InputFloat2("Origin", ref origin)) sprite.Origin = origin;
      }
      var color = sprite.Color.ToNumerics();
      if (ImGui.ColorEdit4("Tint", ref color)) sprite.Color = color;

      var flipBoth = sprite.SpriteEffects == (SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
      var flipH = sprite.SpriteEffects == SpriteEffects.FlipHorizontally || flipBoth;
      var flipV = sprite.SpriteEffects == SpriteEffects.FlipVertically || flipBoth;
      if (ImGui.Checkbox("Flip X", ref flipH)) sprite.SpriteEffects ^= SpriteEffects.FlipHorizontally;
      ImGui.SameLine();
      if (ImGui.Checkbox("Flip Y", ref flipV)) sprite.SpriteEffects ^= SpriteEffects.FlipVertically;
    }
    internal bool _isOpenComponentOptionPopup = false;
    internal SourcedSprite _compOnOptions = null;
    void DrawOptions()
    {
      if (_isOpenComponentOptionPopup)
      {
        _isOpenComponentOptionPopup = false;
        ImGui.OpenPopup("sprite-component-options");
      }
      if (ImGui.BeginPopupContextItem("sprite-component-options") && _compOnOptions != null)
      {
        var lockState = (!_compOnOptions.IsLocked) ? IconFonts.FontAwesome5.LockOpen + "  Unlock" : IconFonts.FontAwesome5.Lock + "  Lock";
        if (ImGui.MenuItem(lockState))
        {
          _compOnOptions.IsLocked = !_compOnOptions.IsLocked;
        }
        var visib = (!_compOnOptions.IsVisible) ? IconFonts.FontAwesome5.EyeSlash + "  Hide" : IconFonts.FontAwesome5.Eye + "  Show";
        if (ImGui.MenuItem(visib))
        {
          _compOnOptions.IsVisible = !_compOnOptions.IsVisible;
        }
        if (ImGui.MenuItem(IconFonts.FontAwesome5.Trash + "  Delete"))
        {
          _compOnOptions.DetachFromSpriteScene();
          if (OnDelPart != null) OnDelPart(_compOnOptions);
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
      ImGui.PushID($"spriteScene-component-{sprite.Name}-options");
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
          SpriteScene.Animations.Add(new Animation(SpriteScene, "Animation"));
        }
        ImGui.EndPopup();
      }
    }
    List<bool> _selectedSprites = new List<bool>();
    List<bool> _selectedAnims = new List<bool>();
    bool _isOpenAnimationOptionPopup = false;
    protected override void OnRenderAfterName()
    {
      _selectedSprites.EqualFalseRange(SpriteScene.Parts.Count());      
      _selectedAnims.EqualFalseRange(SpriteScene.Animations.Count());

      SpriteScene.Transform.RenderImGui();
      var animheader = ImGui.CollapsingHeader("Animations", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.AllowItemOverlap);
      if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
      {
        _isOpenAnimationOptionPopup = true;
      }
      if (animheader)
      {

        ImGui.BeginChild($"spriteScene-anim-child", new System.Numerics.Vector2(ImGui.GetWindowWidth(), 200), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);
        for (int i = 0; i < SpriteScene.Animations.Count(); i++)
        {
          var animation = SpriteScene.Animations[i];
          var isSelected = _selectedAnims[i];
          if (ImGui.Selectable($"{i+1}. {animation.Name}", ref isSelected, ImGuiSelectableFlags.AllowItemOverlap))
          {
            if (!ImGui.GetIO().KeyCtrl) _selectedAnims.FalseRange(_selectedAnims.Count());
            _selectedAnims[i] = true;
            if (OnOpenAnimation != null) OnOpenAnimation(SpriteScene, animation);
          }
        }

        ImGui.EndChild();
      }
      if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding))
      {
        ImGui.BeginChild($"spriteScene-comp-content-child", new System.Numerics.Vector2(ImGui.GetWindowWidth(), 200), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

        SourcedSprite removeSprite = null;
        for (int i = 0; i < _selectedSprites.Count; i++)
        {
          var part = SpriteScene.Parts[i];
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
            ImGui.PushID("spriteScene-component-content-" + part.Name);
            RenderSprite(part);
            ImGui.PopID();

            ImGui.TreePop();
          }

        }
        ImGui.EndChild();
        DrawOptions();
        DrawAnimationOptionPopup();
        if (removeSprite != null) removeSprite.DetachFromSpriteScene();
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