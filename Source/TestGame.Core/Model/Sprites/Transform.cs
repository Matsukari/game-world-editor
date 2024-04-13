using Microsoft.Xna.Framework;
using ImGuiNET;
using Nez;

namespace Raven 
{
  public class Transform 
  {
    public static Transform Default = new Transform();

    public Vector2 Position = new Vector2(0, 0);
    public Vector2 Scale = new Vector2(1, 1);
    public Vector2 Skew = new Vector2(1, 1);

    /// <summary>
    /// In radians
    /// </summary>
    public float Rotation = 0f;
    
    public float RotationNormal { get => (RotationDegrees/360f) % 1f; }

    public float RotationDegrees { get => Mathf.Degrees(Rotation); set => Rotation = Mathf.Radians(value); }

    public Transform() {}
    public void Apply(Nez.Transform transform)
    {
      transform.LocalPosition = Position;
      transform.LocalScale = Scale;
      transform.LocalRotationDegrees = Rotation;
    }
    public bool RenderImGui()
    {
      var mod = false;
      var pos = Position.ToNumerics();
      var scale = Scale.ToNumerics();
      var skew = Skew.ToNumerics();
      var rot = RotationDegrees;
      if (ImGui.InputFloat2("Position", ref pos)) 
      {
        Position = pos.ToVector2();
        mod = true;
      }
      if (ImGui.InputFloat2("Scale", ref scale)) 
      {
        Scale = scale.ToVector2();
        mod = true;
      }
      if (ImGui.InputFloat2("Skew", ref skew)) 
      {
        Skew = skew.ToVector2();
        mod = true;
      }
      if (ImGui.SliderFloat("Rotation", ref rot, 0, 360)) 
      {
        RotationDegrees = rot;
        mod = true;
      }
      return mod;
    }
    public static void RenderImGui(Nez.Transform transform)
    {
      var pos = transform.Position.ToNumerics();
      var scale = transform.Scale.ToNumerics();
      var rot = transform.RotationDegrees;
      if (ImGui.InputFloat2("Position", ref pos)) transform.LocalPosition = pos.ToVector2();
      if (ImGui.InputFloat2("Scale", ref scale)) transform.LocalScale = scale.ToVector2();
      if (ImGui.SliderFloat("Rotation", ref rot, 0, 360)) transform.RotationDegrees = rot;
      // if (ImGui.InputFloat2("Skew", ref skew)) transform.loca = skew.ToVector2();

    }
    public Transform Duplicate()
    {
      Transform transform = new Transform();
      transform.Skew = Skew;
      transform.Position = Position;
      transform.Rotation = Rotation;
      transform.Scale = Scale;
      return transform;
    }
  }
}
