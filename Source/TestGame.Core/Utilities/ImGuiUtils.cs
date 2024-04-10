using ImGuiNET;
using System.Numerics;

namespace Raven 
{
  public class ImGuiUtils
  {
    public static void SpanX(float x)
    {
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(x, 0f));
      ImGui.SameLine();
    }

    public static Vector2 Rotate(Vector2 v, float cos_a, float sin_a) => new Vector2(v.X * cos_a - v.Y * sin_a, v.X * sin_a + v.Y * cos_a);

    public static Vector2 ContainSize(Vector2 size, Vector2 intendedSize)
    {
        float imageRatio = 1f;
        if (size.X > size.Y) 
          imageRatio = intendedSize.X / size.X;
        else 
          imageRatio = intendedSize.Y / size.Y;

        return size * imageRatio;
    }

    public static void TextMiddle(string hint)
    {
      // Calculate the size of the child window
      var childSize = ImGui.GetWindowSize();

      var textPos = new System.Numerics.Vector2();
      textPos.X = (childSize.X - ImGui.CalcTextSize(hint).X) * 0.5f;
      textPos.Y = (childSize.Y - ImGui.GetTextLineHeightWithSpacing()) * 0.5f;

      ImGui.SetCursorPos(textPos);
      ImGui.TextDisabled(hint);
    }

    public static void DrawImage(ImDrawListPtr drawList, nint texture, Vector2 center, Vector2 size, float angle, uint color)
    {
      float cos_a = MathF.Cos(angle);
      float sin_a = MathF.Sin(angle);
      Vector2[] pos = new []
      {
        center + Rotate(new Vector2(-size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
        center + Rotate(new Vector2(+size.X * 0.5f, -size.Y * 0.5f), cos_a, sin_a),
        center + Rotate(new Vector2(+size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a),
        center + Rotate(new Vector2(-size.X * 0.5f, +size.Y * 0.5f), cos_a, sin_a)
      };
      Vector2[] uvs = new []
      { 
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(1.0f, 1.0f), 
        new Vector2(0.0f, 1.0f) 
      };

      drawList.AddImageQuad(texture, pos[0], pos[1], pos[2], pos[3], uvs[0], uvs[1], uvs[2], uvs[3], color);
    }

  }
}
