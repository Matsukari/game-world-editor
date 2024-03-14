using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Raven.Sheet.Sprites 
{
  public class Transform 
  {
    public Vector2 Position = new Vector2(1, 1);
    public Vector2 Scale = new Vector2(1, 1);
    public Vector2 Skew = new Vector2(1, 1);
    public float Rotation = 0f;
    public Transform() {}
    public void Apply(Nez.Transform transform)
    {
      transform.LocalPosition = Position;
      transform.LocalScale = Scale;
      transform.LocalRotationDegrees = Rotation;
    }
    public void RenderImGui()
    {
      var pos = Position.ToNumerics();
      var scale = Scale.ToNumerics();
      var skew = Skew.ToNumerics();
      if (ImGui.InputFloat2("Position", ref pos)) Position = pos.ToVector2();
      if (ImGui.InputFloat2("Scale", ref scale)) Scale = scale.ToVector2();
      if (ImGui.InputFloat2("Skew", ref skew)) Skew = skew.ToVector2();
      ImGui.SliderFloat("Rotation", ref Rotation, 0, 360);

    }
  }
}
