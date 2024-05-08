using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven 
{
  public class SpritePartInspector 
  {
    public static ISceneSprite SpriteBeforeMod { get => _startSprite; }
    static string[] _originTypes = new string[] { "Center", "Topleft", "Custom" };
    static bool _mod = false;
    static ISceneSprite _startSprite;

    static void StartSprite(ISceneSprite sprite)
    {
      _mod = true;
      _startSprite = sprite.Copy(); 
    }
    static public bool DrawComponentOptions(ISceneSprite sprite, ref ISceneSprite removeSprite)
    {
      _mod = false;

      // Options next to name
      var visibState = (!sprite.IsVisible) ? Icon.EyeSlash : Icon.Eye;
      var lockState = (!sprite.IsLocked) ? Icon.LockOpen: Icon.Lock;
      var deleteState = Icon.Times;
      ImGuiUtils.SpanX((ImGui.GetContentRegionMax().X - ImGuiUtils.CalcTextSizeHorizontal(sprite.Name).X - 140));

      ImGui.PushID($"spriteScene-component-{sprite.Name}-options");
      if (ImGui.SmallButton(visibState)) 
      {
        StartSprite(sprite);
        sprite.IsVisible = !sprite.IsVisible;
      }
      ImGui.SameLine();
      if (ImGui.SmallButton(lockState)) 
      {
        StartSprite(sprite);
        sprite.IsLocked = !sprite.IsLocked;
      }
      ImGui.SameLine();
      if (ImGui.SmallButton(deleteState)) 
      {
        StartSprite(sprite);
        removeSprite = sprite;
      }
      ImGui.PopID();

      return _mod;
    }
    public static bool RenderSprite(ImGuiWinManager imgui, ISceneSprite sprite, bool drawName = true)
    {
      string name = sprite.Name;
      _mod = false;

      if (drawName && ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
      {
        StartSprite(sprite);
        sprite.Name = name;
      }

      ImGui.BeginDisabled();
      if (sprite.SourceSprite.Name != "") 
        ImGui.LabelText("Source", sprite.SourceSprite.Name);
      ImGui.LabelText("Region", sprite.SourceSprite.Region.RenderStringFormat());
      ImGui.EndDisabled();

      if (sprite.Transform.RenderImGui())
        StartSprite(sprite);

      var origin = sprite.Origin.ToNumerics();

      var originType = sprite.DeterminePreset();
      // Preset origin
      if (ImGui.Combo("Origin", ref originType, _originTypes, _originTypes.Count()))
      {
        if (originType == 0) 
        {
          StartSprite(sprite);
          sprite.Origin = sprite.SourceSprite.Region.Size.ToVector2()/2f;
        }
        else if (originType == 1) 
        {
          StartSprite(sprite);
          sprite.Origin = new Vector2();
        }
      }
      if (ImGui.InputFloat2("Origin", ref origin)) 
      {
        StartSprite(sprite);
        sprite.Origin = origin;
      }
      var color = sprite.Color.ToNumerics();
      if (ImGui.ColorEdit4("Tint", ref color)) 
      {
        StartSprite(sprite);
        sprite.Color = color;
      }

      var flipBoth = sprite.SpriteEffects == (SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
      var flipH = sprite.SpriteEffects == SpriteEffects.FlipHorizontally || flipBoth;
      var flipV = sprite.SpriteEffects == SpriteEffects.FlipVertically || flipBoth;
      if (ImGui.Checkbox("Flip X", ref flipH)) 
      {
        StartSprite(sprite);
        sprite.SpriteEffects ^= SpriteEffects.FlipHorizontally;
      }
      ImGui.SameLine();
      if (ImGui.Checkbox("Flip Y", ref flipV)) 
      {
        StartSprite(sprite);
        sprite.SpriteEffects ^= SpriteEffects.FlipVertically;
      }

      PropertiesRenderer.Render(imgui, sprite, tree: true);

      return _mod;
    }
  }
  public struct SpriteEffectsImGui : IImGuiRenderable
  {
    public event Action OnModified;
    public SpriteEffects Effects;
    public SpriteEffectsImGui(SpriteEffects effects) => Effects = effects;

    public void Render(ImGuiWinManager imgui)
    {
      var flipBoth = Effects == (SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
      var flipH = Effects == SpriteEffects.FlipHorizontally || flipBoth;
      var flipV = Effects == SpriteEffects.FlipVertically || flipBoth;
      if (ImGui.Checkbox("Flip X", ref flipH)) 
      {
        Effects ^= SpriteEffects.FlipHorizontally;
        if (OnModified != null) OnModified();
      }
      ImGui.SameLine();
      if (ImGui.Checkbox("Flip Y", ref flipV)) 
      {
        Effects ^= SpriteEffects.FlipVertically;
        if (OnModified != null) OnModified();
      }


    }
  }
}
