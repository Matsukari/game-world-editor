using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven 
{
  /// <summary>
  /// ImGui window for editing spriteScene properties
  /// </summary>
  public class SpriteSceneInspector : Widget.PropertiedWindow
  {
    public override string Name { get => SpriteScene.Name; set => SpriteScene.Name = value; }
    public override PropertyList Properties { get => SpriteScene.Properties; set => SpriteScene.Properties = value; }
    public override bool CanOpen => SpriteScene != null;

    public ISceneSprite ChangePart = null;
    ISceneSprite  _spriteScenePart;
    // Gui state data
    public SpriteScene SpriteScene;
    public Vector2 GuiPosition = new Vector2();
    public float GuiZoom = 0.5f;

    public event Action<SpriteScene, Animation> OnOpenAnimation;
    public event Action<ISceneSprite> OnAddPart;
    public event Action<ISceneSprite> OnDelPart;
    public event Action<ISceneSprite> OnModifiedPart;
    public event Action<ISceneSprite> OnEmbedShape;

    static string[] _originTypes = new string[] { "Center", "Topleft", "Custom" };

    public SpriteSceneInspector(SpriteScene spriteScene) 
    {
      SpriteScene = spriteScene;
    }
    public override void OutRender(ImGuiWinManager imgui)
    {
      DrawOptions();
      DrawAnimationOptionPopup();  
    }
        
    public static bool RenderSprite(ImGuiWinManager imgui, ISceneSprite sprite, bool drawName = true)
    {
      string name = sprite.Name;
      bool mod = false;
      if (drawName && ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
      {
        sprite.Name = name;
        mod = true;
      }
      ImGui.BeginDisabled();
        if (sprite.SourceSprite.Name != "") ImGui.LabelText("Source", sprite.SourceSprite.Name);
        ImGui.LabelText("Region", sprite.SourceSprite.Region.RenderStringFormat());
      ImGui.EndDisabled();


      mod = mod || sprite.Transform.RenderImGui();
      var origin = sprite.Origin.ToNumerics();

      var originType = sprite.DeterminePreset();
      // Preset origin
      if (ImGui.Combo("Origin", ref originType, _originTypes, _originTypes.Count()))
      {
        if (originType == 0) sprite.Origin = sprite.SourceSprite.Region.Size.ToVector2()/2f;
        else if (originType == 1) sprite.Origin = new Vector2();
        mod = true;
      }
      if (ImGui.InputFloat2("Origin", ref origin)) 
      {
        mod = true;
        sprite.Origin = origin;
      }
      var color = sprite.Color.ToNumerics();
      if (ImGui.ColorEdit4("Tint", ref color)) sprite.Color = color;

      var flipBoth = sprite.SpriteEffects == (SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
      var flipH = sprite.SpriteEffects == SpriteEffects.FlipHorizontally || flipBoth;
      var flipV = sprite.SpriteEffects == SpriteEffects.FlipVertically || flipBoth;
      if (ImGui.Checkbox("Flip X", ref flipH)) sprite.SpriteEffects ^= SpriteEffects.FlipHorizontally;
      ImGui.SameLine();
      if (ImGui.Checkbox("Flip Y", ref flipV)) sprite.SpriteEffects ^= SpriteEffects.FlipVertically;

      PropertiesRenderer.Render(imgui, sprite);
      PropertiesRenderer.HandleNewProperty(sprite, imgui);


      return mod;
    }
    internal bool _isOpenComponentOptionPopup = false;
    internal bool _isOpenSceneOnSpritePopup = false;
    internal ISceneSprite _compOnOptions = null;
    internal ISceneSprite _copiedSprite; 
    internal ISceneSprite _cutSprite; 
    internal Vector2 _posOnOpenCanvas = Vector2.Zero;

    void DrawOptions()
    {
      if (_isOpenComponentOptionPopup)
      {
        _isOpenComponentOptionPopup = false;
        ImGui.OpenPopup("sprite-component-options");
      }
      if (_isOpenSceneOnSpritePopup)
      {
        _isOpenSceneOnSpritePopup = false;
        ImGui.OpenPopup("scene-canvas-component-options");
      }
      if (ImGui.BeginPopup("sprite-component-options") && _compOnOptions != null)
      {

        if (OnEmbedShape != null && ImGui.MenuItem(Icon.Shapes + "  Embed Shape"))
        {
          OnEmbedShape(_compOnOptions);
        }
        if (_compOnOptions is AnimatedSprite animatedSprite)
        {
          if (ImGui.MenuItem(Icon.Film + "  Open Animation") && OnOpenAnimation != null) OnOpenAnimation(SpriteScene, animatedSprite);
        }

        ImGui.Separator();

        if (ImGui.MenuItem(Icon.LevelDownAlt + "  Send to back"))
        {
          _compOnOptions.SpriteScene.OrderAt(_compOnOptions, 0);  
        }
        if (ImGui.MenuItem(Icon.ChevronDown + "  Move down"))
        {
          _compOnOptions.SpriteScene.BringDown(_compOnOptions);  
        }
        if (ImGui.MenuItem(Icon.ChevronUp + "  Move up"))
        {
          _compOnOptions.SpriteScene.BringUp(_compOnOptions);  
        }
        if (ImGui.MenuItem(Icon.LevelUpAlt + "  Bring to front"))
        {
          _compOnOptions.SpriteScene.OrderAt(_compOnOptions, SpriteScene.Parts.Count-1);  
        }

        ImGui.Separator();

        var lockState = (_compOnOptions.IsLocked) ? Icon.LockOpen + "  Unlock" : Icon.Lock + "  Lock";
        if (ImGui.MenuItem(lockState))
        {
          _compOnOptions.IsLocked = !_compOnOptions.IsLocked;
        }
        var visib = (_compOnOptions.IsVisible) ? Icon.EyeSlash + "  Hide" : Icon.Eye + "  Show";
        if (ImGui.MenuItem(visib))
        {
          _compOnOptions.IsVisible = !_compOnOptions.IsVisible;
        }

        ImGui.Separator();

        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          _compOnOptions.DetachFromSpriteScene();
          if (OnDelPart != null) OnDelPart(_compOnOptions);
        }
        if (ImGui.MenuItem(Icon.Copy + "  Copy"))
        {
          _copiedSprite = _compOnOptions;
        }
        if (ImGui.MenuItem(Icon.Cut + "  Cut"))
        {
          _cutSprite = _compOnOptions;
          _cutSprite.DetachFromSpriteScene();
          if (OnDelPart != null) OnDelPart(_cutSprite);
          _copiedSprite = null;
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate"))
        {
          var sprite = _compOnOptions.SpriteScene.AddSprite(_compOnOptions.Copy());
          sprite.Transform.Position.X += 100;
        }

        ImGui.EndPopup();
      }

      if (ImGui.BeginPopup("scene-canvas-component-options"))
      {
        if ((_cutSprite != null || _copiedSprite != null) && _posOnOpenCanvas != Vector2.Zero && ImGui.MenuItem(Icon.Paste + "  Paste"))
        {
          ISceneSprite gotSprite;
          if (_cutSprite != null)
          {
            gotSprite = _cutSprite;
            _cutSprite = null;
          }
          else 
            gotSprite = _copiedSprite.Copy();

          var sprite = gotSprite.SpriteScene.AddSprite(gotSprite);
          sprite.Transform.Position = _posOnOpenCanvas;
        }
        ImGui.EndPopup();
      }

    }
    void DrawComponentOptions(ISceneSprite sprite, ref ISceneSprite removeSprite)
    {
      // Options next to name
      var visibState = (!sprite.IsVisible) ? Icon.EyeSlash : Icon.Eye;
      var lockState = (!sprite.IsLocked) ? Icon.LockOpen: Icon.Lock;
      var deleteState = Icon.Times;
      ImGuiUtils.SpanX((ImGui.GetContentRegionMax().X - ImGuiUtils.CalcTextSizeHorizontal(sprite.Name).X - 140));

      ImGui.PushID($"spriteScene-component-{sprite.Name}-options");
      if (ImGui.SmallButton(visibState)) sprite.IsVisible = !sprite.IsVisible;
      ImGui.SameLine();
      if (ImGui.SmallButton(lockState)) sprite.IsLocked = !sprite.IsLocked;
      ImGui.SameLine();
      if (ImGui.SmallButton(deleteState)) removeSprite = sprite;
      ImGui.PopID();

    }
    void DrawAnimationOptionPopup()
    {
      if (_isOpenAnimationOptionPopup)
      {
        _isOpenAnimationOptionPopup = false;
        ImGui.OpenPopup("animation-options-popup");
      }
      if (_isOpenAnimationOperations)
      {
        _isOpenAnimationOperations = false;
        ImGui.OpenPopup("animation-operations-popup");
      }

      if (ImGui.BeginPopup("animation-options-popup"))
      {
        if (ImGui.MenuItem("Create Animation"))
        {
          SpriteScene.AddAnimation(new Animation(SpriteScene));
        }
        ImGui.EndPopup();
      }
      if (ImGui.BeginPopup("animation-operations-popup") && _onOpenAnimtaionOperations != null)
      {
        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          SpriteScene.RemoveAnimation(_onOpenAnimtaionOperations.Name);
          _onOpenAnimtaionOperations = null;
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate"))
        {
          SpriteScene.AddAnimation(_onOpenAnimtaionOperations.Copy());
        }
        ImGui.EndPopup();
      }
    }
    List<bool> _selectedSprites = new List<bool>();
    List<bool> _selectedAnims = new List<bool>();
    bool _isOpenAnimationOptionPopup = false;
    bool _isOpenAnimationOperations = false;
    Animation _onOpenAnimtaionOperations;

    protected override void OnRenderAfterName(ImGuiWinManager imgui)
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

        if (SpriteScene.Animations.Count == 0)
          ImGuiUtils.TextMiddle("No animations yet.");

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
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
          {
            _isOpenAnimationOperations = true;
            _onOpenAnimtaionOperations = animation;
          }
        }

        ImGui.EndChild();
      }
      ISceneSprite removeSprite = null;
      if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.FramePadding))
      {
        ImGui.BeginChild($"spriteScene-comp-content-child", new System.Numerics.Vector2(ImGui.GetWindowWidth(), 300), 
            false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

        if (_selectedSprites.Count == 0)
          ImGuiUtils.TextMiddle("No components found.");

        for (int i = _selectedSprites.Count()-1; i >= 0; i--)
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
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
          {
            _isOpenComponentOptionPopup = true;
            _compOnOptions = part;
          }

          DrawComponentOptions(part, ref removeSprite);

          if (spriteNode)
          {
            ImGui.PushID("spriteScene-component-content-" + part.Name);
            if (RenderSprite(imgui, part)) OnModifiedPart(part);
            ImGui.PopID();

            ImGui.TreePop();
          }

        }
        ImGui.EndChild();
      }
      if (removeSprite != null) removeSprite.DetachFromSpriteScene();
    }
    public override string GetIcon()
    {
      return Icon.User;
    }
    protected override void OnChangeName(string old, string now)
    {
    } 
  }
}
