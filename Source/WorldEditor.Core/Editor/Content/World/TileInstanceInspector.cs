using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Raven
{
  public abstract class ClassMemberChanger 
  {
    object _obj;
    string _memberName;
    
    FieldInfo GetField() => _obj.GetType().GetField(_memberName);
    PropertyInfo GetProperty() => _obj.GetType().GetProperty(_memberName);

    public void Init(object obj, string member)
    {
      _obj = obj;
      _memberName = member;
    }

  }
  // public class TransformImGuiRenderer : ClassMemberChanger, IImGuiRenderable
  // { 
  //   Transform _transform;
  //   public void Render(ImGuiWinManager imgui)
  //   {
  //     var mod = false;
  //     var pos = _transform.Position.ToNumerics();
  //     var scale = _transform.Scale.ToNumerics();
  //     var skew = _transform.Skew.ToNumerics();
  //     var rot = RotationDegrees;
  //     if (ImGui.InputFloat2("Position", ref pos)) 
  //     {
  //       Position = pos.ToVector2();
  //       mod = true;
  //     }
  //     if (ImGui.InputFloat2("Scale", ref scale)) 
  //     {
  //       Scale = scale.ToVector2();
  //       mod = true;
  //     }
  //     if (ImGui.InputFloat2("Skew", ref skew)) 
  //     {
  //       Skew = skew.ToVector2();
  //       mod = true;
  //     }
  //     if (ImGui.SliderFloat("Rotation", ref rot, 0, 360)) 
  //     {
  //       RotationDegrees = rot;
  //       mod = true;
  //     }
  //     return mod;
  //   }
  // }
  public class TileInstanceInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Tile.Name; set => Tile.Name = value;}
    public override PropertyList Properties { get => Tile.Properties; set => Tile.Properties = value; }
    public override bool CanOpen => Tile != null && Layer != null;
        
    public TileInstance Tile;
    public TileLayer Layer;
    public event Action<TileInstance, TileLayer> OnTileModified;

    static bool _mod = false;
    static RenderProperties _startProps;
    static void StartProps(TileInstance sprite)
    {
      _mod = true;
      if (sprite.Props == null) _startProps = null;
      else _startProps = sprite.Props.Copy(); 
      Console.WriteLine("Null :" + sprite.Props == null);
    }
    bool _onRelease = false;
    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      ImGui.BeginDisabled();
      ImGui.LabelText("Id", Tile.Tile.Id.ToString());
      ImGui.LabelText("Tile", $"{Tile.Tile.Coordinates.X}x, {Tile.Tile.Coordinates.Y}y");
      ImGui.EndDisabled();
      if (Tile.Props == null)
      {
        if (ImGui.Button(IconFonts.FontAwesome5.Plus + "   Add Custom Render Attributes"))
        {
          StartProps(Tile);
          Tile.Props = new RenderProperties();
        }
      }
      else 
      {
        if (Tile.Props.Transform.RenderImGui() && OnTileModified != null) OnTileModified(Tile, Layer);

        var color = Tile.Props.Color.ToNumerics();
        
        if (ImGui.ColorEdit4("Tint", ref color)) 
        {
          _onRelease = true;
          StartProps(Tile);
          Tile.Props.Color = color;
        }

        var flipBoth = Tile.Props.SpriteEffects == (SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
        var flipH = Tile.Props.SpriteEffects == SpriteEffects.FlipHorizontally || flipBoth;
        var flipV = Tile.Props.SpriteEffects == SpriteEffects.FlipVertically || flipBoth;
        if (ImGui.Checkbox("Flip X", ref flipH)) 
        {
          StartProps(Tile);
          Tile.Props.SpriteEffects ^= SpriteEffects.FlipHorizontally;
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("Flip Y", ref flipV)) 
        {
          StartProps(Tile);
          Tile.Props.SpriteEffects ^= SpriteEffects.FlipVertically;
        }
      } 
      if (_mod && (!_onRelease || Nez.Input.LeftMouseButtonReleased))
      {
        _onRelease = false;
        Nez.Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RenderPropModifyCommand(Tile, "Props", Tile.Props, _startProps));
        _mod = false;
      }
    } 
  }
}

